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
    // Responsible for running an insert query
    // Doesn't really do much, since the majority
    // of the work is in writing out the new row
    // to disk
    // <summary>
    internal class Inserter : IRunnable
    {
        NetBase.Sql.InsertQuery Query { get; set; }

        public Inserter(NetBase.Sql.InsertQuery q)
        {
            this.Query = q;
        }


        public ITable Execute(Database db)
        {
            ITable t = db.Tables[Query.TableName];
            Row nr = new Row(t);
            foreach (Set s in Query.Sets)
            {
                nr[s.Field] = s.Value;
            }

            t.AddRow(nr);
            return null;
        }
    }
}
