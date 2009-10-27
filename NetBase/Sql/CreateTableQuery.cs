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
    // Clearly a query that creates a table. So for this we
    // need a list of fields and a table name. At the moment
    // the only datatype we support is a string, but we're preparing
    // for datatypes with an inner class.
    // </summary>
    internal class CreateTableQuery : IQuery
    {
        public class CreateField
        {
            public string Name;
        }

        public string TableName { get; set; }
        public List<CreateField> Fields {get;set;}

        public CreateTableQuery()
        {
            this.Fields = new List<CreateField>();
        }

        public IRunnable Runnable { get { return new TableCreater(this); } }
    }
}
