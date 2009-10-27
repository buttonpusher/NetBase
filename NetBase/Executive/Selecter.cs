/*
NetBase .NET database
Copyright (C) 2009  buttonpusher

This program is free software; you can redistribute it and/or
modify it under the terms of the GNU General Public License
as published by the Free Software Foundation; either version 2
of the License, or (at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program; if not, write to the Free Software
Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NetBase.Executive;
using NetBase.Storage;
using NetBase.Sql;

namespace NetBase.Executive
{
    // <summary>
    // Couldn't decide whether to call this "Selecter" or "Selector".
    // Regardless, it runs the select query, which is quite
    // complicated and only going to get bigger. So there may be
    // some design issues which need to be addresses here as we 
    // really start getting to the key functionality.
    // </summary>
    internal class Selecter : IRunnable
    {
        NetBase.Sql.SelectQuery Query { get; set; }

        public Selecter(NetBase.Sql.SelectQuery q)
        {
            this.Query = q;
        }

        // <summary>
        // Naive implementation of a select with join support
        // Not likely to perform well for a Fortune 500 financial
        // institution, but it's a start.
        // </summary>
        public ITable Execute(Database db)
        {
            // Somewhere to store the data.
            MemoryTable nt = new MemoryTable();

            ITable t = db.Tables[Query.TableName];

            // We're going to start by building a column
            // list for our new tables.
            if (Query.SelectAll)
            {
                // @TODO these should be deep copies
                foreach (Column c in t.Columns)
                {
                    nt.Columns.Add(c);
                }

                foreach (Join j in Query.Joins)
                {
                    ITable jt = db.Tables[j.TableName];
                    foreach (Column c in jt.Columns)
                    {
                        nt.Columns.Add(c);
                    }
                }
            }
            else
            {
                // Uh-oh, we haven't implemented support
                // for joins when using named fields.
                foreach (string s in Query.Fields)
                {
                    Column c = t.Columns.Find(o => o.Name == s);
                    nt.Columns.Add(c);
                }
            }

            // "High performance NetBase" is a book yet to
            // be written. We're going to do a full table
            // seek every time, and we don't have any support
            // for indexes.
            t.Reset();
            while (t.HasRows)
            {
                Row r = t.NextRow();
                bool pass = true;
                foreach (Where w in Query.Wheres)
                {
                    if (!w.Filter(r))
                    {
                        pass = false;
                        break;
                    }
                }

                if (pass)
                {
                    Row nr = new Row(nt);
                    nr.CopyData(r, nt.Columns);

                    // Now copy data from joined rows.
                    foreach (Join j in Query.Joins)
                    {
                        // Lazily, we have only implemented left outer joins.
                        // This is at proof-of-concept stage.
                        if (j.GetType() != typeof(OuterJoin)
                            || j.Orientation == Join.OrientationType.Right)
                        {
                            throw new SQLUnsupportedException("SQL is valid, but the functionality is unsupported (only left outer joins supported)");
                        }

                        // find everything in the other table that matches
                        // and add it to the final table
                        ITable joined = db.Tables[j.TableName];
                        joined.Reset();
                        while (joined.HasRows)
                        {
                            Row jr = joined.NextRow();
                            string left = (string)jr[j.RightField];
                            string right = (string)r[j.LeftField];
                            if (left == right)
                            {
                                nr.CopyData(jr, joined.Columns);
                            }
                        }
                    }

                    nt.AddRow(nr);
                }
            }
            return nt;
        }
    }
}
