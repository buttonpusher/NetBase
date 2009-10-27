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

namespace NetBase.Storage
{
    //<summary>Nice utility class for the database
    //caches tables in memory so we're not constantly opening
    // and closing them</summary>
    internal class TableSet : IDisposable
    {
        private System.Collections.Hashtable _tableCache = new System.Collections.Hashtable();
        private Database _database;

        public TableSet(Database owner)
        {
            this._database = owner;
        }

        internal ITable this[string tableName]
        {
            get
            {
                string fullpath = Path.Combine(_database.Directory, tableName + ".dbf");

                if (_tableCache.Contains(fullpath))
                {
                    return (ITable)_tableCache[fullpath];
                }

                ITable t = new FileTable(fullpath);
                _tableCache.Add(fullpath, t);
                return t;
            }
        }

        internal ITable Add(Sql.CreateTableQuery q)
        {
            FileTable ft = new FileTable();
            foreach (Sql.CreateTableQuery.CreateField field in q.Fields)
            {
                Column item = new Column();
                item.Name = field.Name;
                item.DataType = 'C';
                ft.Columns.Add(item);
            }
            string filename = Path.Combine(_database.Directory, q.TableName + ".DBF");
            ft.CommitToDisk(filename);
            return (ITable)ft;
        }

        #region IDisposable Members

        public void Dispose()
        {
            foreach (object o in _tableCache.Values)
            {
                ((ITable)o).Dispose();
            }
        }

        #endregion
    }
}
