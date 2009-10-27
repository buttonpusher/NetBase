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

namespace NetBase.Sql
{
    // <summary>
    // Represents a join. It's an abstract class because the sql parser
    // (QueryBuilder) now supports inner and outer joins, although the
    // executive that actually runs the queries does not.
    // </summary>
    internal abstract class Join
    {
        // <summary>
        // For consistency,
        // we could subclass inner/outer into left and right, but that
        // might be going over the top; so for now we use an enum.
        // </summary>
        public enum OrientationType
        {
            Left,
            Right
        }

        public OrientationType Orientation { get; set; }
        public string TableName { get; set; }
        public string LeftField { get; set; }
        public string RightField { get; set; }
    }
}
