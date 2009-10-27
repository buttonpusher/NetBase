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
    // <summary>This class stores info about the table. You can really
    // see the dbf heritage here... everything's an Int16. No bounds
    // checking so some really small hard limits
    // </summary>
    internal class TableHeader
    {
        public Int16 RowCount { get; set; }
        public Int16 Size { get; set; }
        public Int16 RowLength { get; set; }
        
        public int ColumnCount
        {
            get
            {
                return (this.Size / 32) - 1;
            }
        }

        public void BinRead(BinaryReader br)
        {
            br.ReadBytes(4);
            this.RowCount = BitConverter.ToInt16(br.ReadBytes(2), 0);
            br.ReadBytes(2);
            this.Size = BitConverter.ToInt16(br.ReadBytes(2), 0);
            this.RowLength = BitConverter.ToInt16(br.ReadBytes(2), 0);
            br.ReadBytes(20);
        }

        public void BinWrite(BinaryWriter bw)
        {
            bw.Write(new byte[4]);
            bw.Write(BitConverter.GetBytes(this.RowCount));
            bw.Write(new byte[2]);
            bw.Write(BitConverter.GetBytes(this.Size));
            bw.Write(BitConverter.GetBytes(this.RowLength));
            bw.Write(new byte[20]);
        }
    }
}
