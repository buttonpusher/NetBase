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
    // Unfortunately actually executing a select statement is
    // quite a challenge; and such a large bit of code that
    // it seems a bit out of place. Thus the "executive" namespace
    // that actually deals with executing the queries; which may
    // mean it would be a good idea to clean up the parsing by moving
    // it into individual classes.
    // </summary>
    internal class SelectQuery : IQuery
    {
        public List<Where> Wheres { get; set; }
        public List<string> Fields {get; set; }
        public List<Join> Joins { get; set; }
        public string TableName { get; set; }
        public bool SelectAll = false;

        public SelectQuery()
        {
            this.Wheres = new List<Where>();
            this.Fields = new List<string>();
            this.Joins = new List<Join>();
        }

        public IRunnable Runnable
        {
            get { return new Selecter(this); }
        }
    }
}
