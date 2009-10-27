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
using System.Data.Common;

namespace NetBase.Api
{
    public class DbfConnection : DbConnection
    {
        internal Storage.Database UnderlyingDatabase { get; set; }

        public DbfConnection()
        {
            this.UnderlyingDatabase = new Storage.Database();
        }

        protected override DbTransaction BeginDbTransaction(System.Data.IsolationLevel isolationLevel)
        {
            throw new NotImplementedException();
        }

        public override void ChangeDatabase(string databaseName)
        {
            throw new NotImplementedException();
        }

        public override void Close()
        {
            if (UnderlyingDatabase != null)
            {
                UnderlyingDatabase.Dispose();
            }
        }

        public override string ConnectionString
        {
            get
            {
                return this.UnderlyingDatabase.Directory;
            }
            set
            {
                if (!System.IO.Directory.Exists(value))
                {
                    throw new Exception("Cannot set connection string as directory doesn't exist");
                }
                this.UnderlyingDatabase.Directory = value;
            }
        }

        protected override DbCommand CreateDbCommand()
        {
            return new DbfCommand(this);
        }

        public override string DataSource
        {
            get { throw new NotImplementedException(); }
        }

        public override string Database
        {
            get { throw new NotImplementedException(); }
        }

        public override void Open()
        {
            // Does nothing, because the underlying IO is on-demand
        }

        public override string ServerVersion
        {
            get { throw new NotImplementedException(); }
        }

        public override System.Data.ConnectionState State
        {
            get { throw new NotImplementedException(); }
        }
    }
}
