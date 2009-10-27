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
    // <summary>
    // The column class is roughly based on a DBF style struct.
    // Names are limited to ten characters, there are a couple
    // of datatypes; but they're all written as strings, that kind
    // of thing. Note that NetBase currently only supports writing
    // strings; although it can read different datatypes
    // See the table class for more notes about the on-disk format.
    // </summary>
    internal class Column
    {
        private char _dataType;
        private string _name;

        public char DataType
        {
            get
            {
                return this._dataType;
            }
            set
            {
                // @TODO data type exception
                if (value != 'C') throw new UnsupportedDataTypeException("Cannot write datatypes other than strings");
                this._length = 10; // length of a string
                this._dataType = value;
            }
        }

        public string Name 
        {
            get
            {
                return this._name;
            }
            set
            {
                if (value.Length > 10)
                {
                    // @TODO invalid datatype exception
                    throw new InvalidDataException("Cannot set column names greater than 10 characters long");
                }
                this._name = value;
            }
        }

        private Int16 _length;
        //<summary> The length is calculated when we write a new column,
        // but read when we open an existing table</summary>
        public Int16 Length { get { return this._length; } }

        
        public void BinRead(BinaryReader br)
        {
            // @TODO this is just plain lazily inefficient, could
            // cache the ASCIIEncoding.
            ASCIIEncoding ae = new ASCIIEncoding();
            this.Name = ae.GetString(br.ReadBytes(10)).Trim('\0').Trim();
            br.ReadBytes(1);
            this.DataType = ae.GetString(br.ReadBytes(1))[0];
            br.ReadBytes(4);
            this._length = (Int16)br.ReadByte();
            br.ReadBytes(15);
        }

        public void BinWrite(BinaryWriter bw)
        {
            ASCIIEncoding ae = new ASCIIEncoding();
            byte[] names = new byte[10];
            for (int i = 0; i < names.Length; i++)
            {
                names[i] = (byte)'\0';
            }
            ae.GetBytes(this.Name).CopyTo(names, 0);
            byte[] dt = ae.GetBytes(new char[] { this.DataType });

            // Space for two bytes for length, but we're only
            // going to use one.
            byte[] len = BitConverter.GetBytes(this.Length);
            bw.Write(names);
            bw.Write(new byte[1]);
            bw.Write(dt);
            bw.Write(new byte[4]);
            bw.Write(len);
            // here's where we skip the extra byte we put in for length
            bw.Write(new byte[14]);
        }

        // <summary>The column knows about the datatype, so it
        // takes care of reading and writing the row's data
        // </summary>
        public void WriteToRow(BinaryWriter bw, string data)
        {
            ASCIIEncoding ae = new ASCIIEncoding();
            byte[] buff = new byte[this.Size];
            ae.GetBytes(data,0,data.Length,buff,0);
            bw.Write(buff);
        }

        // <summary>The column knows about the datatype, so it
        // takes care of reading and writing the row's data
        // </summary>
        public object ReadFromRow(BinaryReader br)
        {
            ASCIIEncoding ae = new ASCIIEncoding();
            return ae.GetString(br.ReadBytes(this.Length)).Trim('\0').Trim();
        }
        // <summary>Size of the column will eventually be different
        // from length, e.g. memo/varchar fields.
        // </summary>
        public int Size
        {
            get
            {
                switch (this.DataType)
                {
                    case 'C':
                        {
                            return this.Length;
                        }
                    case 'N':
                        {
                            return this.Length;
                        }
                    case 'D':
                        {
                            return 8;
                        }
                    case 'L':
                        {
                            return 1;
                        }
                    case 'M':
                        {
                            return 10;
                        }
                    default:
                        throw new Exception("Unknown datatype");
                }
            }

        }
    }
}
