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

namespace NetBase.Executive
{
    // <summary>
    // The parser has got ahead of the executive. We throw this
    // to indicate that parsing has been successful, but we
    // can't actually run it...
    // </summary>
    class SQLUnsupportedException : Exception
    {
        public SQLUnsupportedException(string message)
            : base(message)
        {
        }
    }
}
