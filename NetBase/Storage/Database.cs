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
using System.IO;
using NetBase.Sql;
using NetBase.Executive;

namespace NetBase.Storage
{
    // <summary>Represents a database, which is really just
    // a folder with a bunch of files in it.
    // </summary>
    internal class Database : IDisposable
    {
        public string Directory { get; set; }
        public TableSet Tables { get; set; }

        public Database()
        {
            this.Tables = new TableSet(this);
        }

        // <summary>Here for convenience, very much reflecting
        // the structure of the ADO.NET stuff</summary>
        public ITable Execute(string stmt)
        {
            QueryBuilder qb = new QueryBuilder();
            IQuery q = qb.Parse(stmt);
            
            return this.Execute(q.Runnable);
        }

        public ITable Execute(IRunnable q)
        {
            return q.Execute(this);
        }

        #region IDisposable Members
        // <summary> We need to close all our tables</summary>
        public void Dispose()
        {
            this.Tables.Dispose();
        }

        #endregion
    }
}
