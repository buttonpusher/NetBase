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
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace NetBase.Storage
{
    // <summary>An on-disk table. This is loosely based on DBF of course,
    // you can probably use it to read them. However, a lot of the fields
    // are left empty - e.g. data types, etc. etc.
    // These classes might make a good candidate for implementing IEnumerator
    // </summary>
    internal class FileTable : ITable
    {
        private BinaryReader _br;
        private FileStream _fs;
        private int _currentRow = 0;
        private TableHeader Header { get; set; }
        public List<Column> Columns { get; set; }

        // <summary>This is really only needed for sending data over the network
        // in which case the results will (currently) always be the results of
        // a query, and therefore always a MemoryTable</summary>
        public byte[] ToByteArray()
        {
            throw new NotImplementedException();
        }

        public FileTable()
        {
            this.Columns = new List<Column>();
        }

        // <summary>Rewind the table back to the beginning</summary>
        public void Reset()
        {
            this._fs.Seek(this.Header.Size, 0);
            this._currentRow = 0;
        }

        // <summary>Does what it says on the tin - does the table have any rows</summary>
        public bool HasRows
        {
            get
            {
                return this._currentRow <= this.Header.RowCount;
            }
        }

        // <summary>Retrieve the next row. Uses a seek,
        // assuming the underlying framework will be sensible about it
        // (we should usually already be in position)</summary>
        public Row NextRow()
        {
            _fs.Seek(this.Header.Size + (this._currentRow * this.Header.RowLength),0);
            Row r = new Row(this);
            r.BinRead(this.Columns, this._br);
            this._currentRow++;
            return r;
        }

        // <summary>Opens up a file handle on the table data
        // and reads in the header. Note the file is held
        // open until the FileTable is disposed.</summary>
        public FileTable(string filename)
        {
            this.Open(filename);
        }

        private void Open(string filename)
        {
            this._fs = new FileStream(filename, FileMode.Open);
            this._br = new BinaryReader(this._fs);

            this.Header = new TableHeader();
            this.Header.BinRead(this._br);

            this.Columns = new List<Column>();
            for (Int16 i = 0; i < this.Header.ColumnCount; i++)
            {
                Column c = new Column();
                c.BinRead(_br);
                this.Columns.Add(c);
            }

            // Read mysterious extra byte; found in original test
            // table from a Lotus Approach database.
            _br.ReadByte();
            // Sanity check row size
            if (this.CalculateTotalRowSize() != this.Header.RowLength)
            {
                throw new BadDataException("Total size of columns does not match that defined in the header");
            }

        }

        // For sanity checking, writing etc.
        public int CalculateTotalRowSize()
        {
            int size = 1; // add one for deleted flag
            foreach (Column c in Columns)
            {
                size += c.Size;
            }
            return size;
        }

        // <summary>
        // This method is intended to be used only when
        // creating a new table. So; e.g. not for normal writes.
        // </summary>
        public void CommitToDisk(string filename)
        {
            // Check that the file doesn't already exist
            if (File.Exists(filename))
            {
                throw new InvalidOperationException("Commit is for writing new tables only; and the filename allocated for this table already exists");
            }

            // Use a temporary handle, it will be re-opened when we retrieve from it
            using (FileStream newTableStream = new FileStream(filename, FileMode.CreateNew))
            {
                BinaryWriter bw = new BinaryWriter(newTableStream);
                // Write the header
                this.Header = new TableHeader();
                this.Header.Size = (short)((this.Columns.Count + 1) * 32);
                this.Header.RowCount = 0;
                this.Header.RowLength = (short)this.CalculateTotalRowSize();
                this.Header.BinWrite(bw);
                foreach (Column c in this.Columns)
                {
                    c.BinWrite(bw);
                }
                // Add mysterious extra byte
                bw.Write(new byte[1]);

                bw.Close();
            }

            // now immediately open the file
            Open(filename);
        }

        // <summary>
        // Yes, I am sure there is room for optimisation here...
        // </summary>
        public List<Row> Find(Predicate<Row> p)
        {
            this.Reset();

            List<Row> results = new List<Row>();
            while (this.HasRows)
            {
                Row r = this.NextRow();

                if (p(r))
                {
                    results.Add(r);
                }
            }
            return results;
        }

        // <summary>
        // Writes a row to disk
        // </summary>
        public void AddRow(Row r)
        {
            if (r.Table != this)
            {
                throw new InvalidOperationException("Can't add row - doesn't belong to this table");
            }
            
            _fs.Seek(_fs.Length, 0);
            BinaryWriter bw = new BinaryWriter(_fs);
            r.BinWrite(bw);

            this.Header.RowCount += 1;
            _fs.Seek(0, 0);
            this.Header.BinWrite(bw);

            bw.Flush();
        }

        public int RowCount
        {
            get
            {
                return this.Header.RowCount;
            }
        }

        #region IDisposable Members

        public void Dispose()
        {
            this._br.Close();
            this._fs.Dispose();
        }

        #endregion
    }
}
