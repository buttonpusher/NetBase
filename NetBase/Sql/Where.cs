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
using NetBase.Storage;

namespace NetBase.Sql
{
    // <summary>
    // Stores details of where clauses. Actually, having the filter
    // function in here is not consistent with the rest of the query
    // engine, where the actual functionality is in the "executive" namespace
    // </summary>
    internal class Where
    {
        public string Field;
        public string Value;

        public bool Filter(Row r)
        {
            if ((string)r[Field] != Value)
            {
                return false;
            }
            return true;
        }
    }
}
