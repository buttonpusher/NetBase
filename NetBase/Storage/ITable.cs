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
namespace NetBase.Storage
{
    // <summary>Most important interface. Describes datasets
    // Could be expanded to allow for pluggable storage engines
    // </summary>
    internal interface ITable : IDisposable
    {
        void AddRow(Row r);
        System.Collections.Generic.List<Column> Columns { get; set; }
        System.Collections.Generic.List<Row> Find(Predicate<Row> p);
        bool HasRows { get; }
        Row NextRow();
        void Reset();
        int RowCount { get; }
        byte[] ToByteArray();
    }
}
