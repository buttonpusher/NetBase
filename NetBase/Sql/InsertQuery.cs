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
using NetBase.Executive;

namespace NetBase.Sql
{
    // <summary>
    // An insert requires a set of fields and a tablename. I plan
    // on using the same "Set" class for updates, which is why it's
    // called Set.
    // <summary>
    internal class InsertQuery : IQuery
    {
        public List<Set> Sets { get; set; }
        public string TableName { get; set; }
        
        public InsertQuery()
        {
            this.Sets = new List<Set>();
        }

        public IRunnable Runnable { get { return new Inserter(this); } }
    }
}
