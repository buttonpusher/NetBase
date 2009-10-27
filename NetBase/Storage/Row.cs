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

namespace NetBase.Storage
{
    // <summary>Stores data.</summary>
    internal class Row
    {
        public ITable Table { get; set; }
        public bool Deleted { get; set; }

        private object[] _data;

        public Row(ITable t)
        {
            this.Table = t;
            this._data = new object[t.Columns.Count];
        }

        public object this[int index]
        {
            get
            {
                return _data[index];
            }
        }

        public object this[string name]
        {
            get
            {
                int index = this.Table.Columns.FindIndex(c => c.Name == name);
                if (index == -1) throw new ArgumentException("Invalid column name \"" + name + "\"");
                return this[index];
            }
            set
            {
                int index = this.Table.Columns.FindIndex(c => c.Name == name);
                if (index == -1) throw new ArgumentException("Invalid column name \"" + name + "\"");
                _data[index] = value;
            }
        }

        // <summary>Generally used for creating new rows out of
        // old ones, particularly when joining rows together</summary>
        public void CopyData(Row old, List<Column> newColumns)
        {
            object[] oldd = this._data;
            this._data = new object[this.Table.Columns.Count];
            

            oldd.CopyTo(this._data, 0);

            foreach (Column c in newColumns)
            {
                // Check if the column exists on this table,
                // if it does copy the value in
                if (this.Table.Columns.Find(o => o.Name == c.Name) != null &&
                    old.Table.Columns.Find(o => o.Name == c.Name) != null)
                {
                
                    // find the matching old column
                    this[c.Name] = old[c.Name];
                }
                
            }
        }

        public void BinRead(List<Column> columns, System.IO.BinaryReader br)
        {
            this._data = new object[columns.Count];
            
            // Read deleted field
            br.ReadBytes(1);

            int counter = 0;
            foreach (Column c in columns)
            {
                _data[counter] = c.ReadFromRow(br);
                counter++;
            }
        }

        public void BinWrite(System.IO.BinaryWriter bw)
        {
            ASCIIEncoding ae = new ASCIIEncoding();

            // Write deleted bytes
            bw.Write((byte)0);
            int counter = 0;
            foreach (Column c in this.Table.Columns)
            {
                object data = this._data[counter];
                if (data == null)
                {
                    data = "";
                }
                c.WriteToRow(bw, (string)data);
                counter++;
            }
        }

        public override string ToString()
        {
            return (string)this._data.Aggregate<object>((o,e) => (string)o+(string)e);
        }
    }
}
