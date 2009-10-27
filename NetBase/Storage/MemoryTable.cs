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
    //<summary>Store dataset in memory. Mainly used for
    // returning the results of queries.
    //</summary>
    internal class MemoryTable : ITable
    {
        public List<Column> Columns { get; set; }
        private TableHeader Header { get; set; }
        private List<Row> Rows { get; set; }
        private int _currentRow = 0;

        public MemoryTable()
        {
            this.Columns = new List<Column>();
            this.Rows = new List<Row>();
            this.Header = new TableHeader();

        }

        //<summary>The intention of this method is to use it
        //to send data over the wire. to a network client</summary>
        public byte[] ToByteArray()
        {
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            System.IO.BinaryWriter bw = new System.IO.BinaryWriter(ms);
            this.Header.BinWrite(bw);
            foreach (Column c in this.Columns)
            {
                c.BinWrite(bw);
            }
            while (this.HasRows)
            {
                Row r = this.NextRow();
                r.BinWrite(bw);
            }
            bw.Flush();
            bw.Close();
            return ms.GetBuffer();
        }

        #region ITable Members


        public void AddRow(Row r)
        {
            if (r.Table != this)
            {
                throw new Exception("Cannot add row - doesn't belong to this table");
            }
            this.Rows.Add(r);
        }

        
        public List<Row> Find(Predicate<Row> p)
        {
            return this.Rows.FindAll(p);
        }

        public bool HasRows
        {
            get { return this._currentRow < this.Rows.Count; }
        }

        public Row NextRow()
        {
            this._currentRow++;
            return this.Rows[this._currentRow - 1];
        }

        public void Reset()
        {
            this._currentRow = 0;
        }

        public int RowCount
        {
            get { return this.Rows.Count; }
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            
        }

        #endregion
    }
}
