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
using NUnit.Framework;
using NetBase.Sql;

namespace NetBase.Tests.Sql
{
    [TestFixture]
    public class QueryBuilderTest
    {
        private NetBase.Sql.QueryBuilder qb;

        [TestFixtureSetUp]
        public void SetUp()
        {
            qb = new QueryBuilder();
        }

        [Test]
        public void SelectStarNoWheres()
        {
            string stmt = "SELECT * FROM MyTable";
            IQuery query = qb.Parse(stmt);

            Assert.AreEqual(typeof(SelectQuery), query.GetType());
            Assert.AreEqual("MyTable", query.TableName);
            Assert.AreEqual(true, ((SelectQuery)query).SelectAll);
            Assert.AreEqual(0, ((SelectQuery)query).Fields.Count);
            Assert.AreEqual(0, ((SelectQuery)query).Wheres.Count);
        }

        [Test]
        public void SelectStarOneWhere()
        {
            string stmt = "SELECT * FROM MyTable WHERE MyField = 'True'";
            IQuery query = qb.Parse(stmt);

            Assert.AreEqual(typeof(SelectQuery), query.GetType());
            Assert.AreEqual("MyTable", query.TableName);
            Assert.AreEqual(true, ((SelectQuery)query).SelectAll);
            Assert.AreEqual(0, ((SelectQuery)query).Fields.Count);
            Assert.AreEqual(1, ((SelectQuery)query).Wheres.Count);
            Assert.AreEqual("MyField",((SelectQuery)query).Wheres[0].Field);
            Assert.AreEqual("True",((SelectQuery)query).Wheres[0].Value);
        }

        [Test]
        public void SelectStarTwoWhere()
        {
            string stmt = "SELECT * FROM MyTable WHERE MyField = 'True' AND OtherField = 'False'";
            IQuery query = qb.Parse(stmt);

            Assert.AreEqual(typeof(SelectQuery), query.GetType());
            Assert.AreEqual("MyTable", query.TableName);
            Assert.AreEqual(true, ((SelectQuery)query).SelectAll);
            Assert.AreEqual(0, ((SelectQuery)query).Fields.Count);
            Assert.AreEqual(2, ((SelectQuery)query).Wheres.Count);
            Assert.AreEqual("MyField", ((SelectQuery)query).Wheres[0].Field);
            Assert.AreEqual("True", ((SelectQuery)query).Wheres[0].Value);
            Assert.AreEqual("OtherField", ((SelectQuery)query).Wheres[1].Field);
            Assert.AreEqual("False", ((SelectQuery)query).Wheres[1].Value);
        }

        [Test]
        public void SelectStarMultipleWhere()
        {
            string stmt = "SELECT * FROM MyTable WHERE MyField = 'True' AND OtherField = 'False' AND finalField='nothing'";
            IQuery query = qb.Parse(stmt);

            Assert.AreEqual(typeof(SelectQuery), query.GetType());
            Assert.AreEqual("MyTable", query.TableName);
            Assert.AreEqual(true, ((SelectQuery)query).SelectAll);
            Assert.AreEqual(0, ((SelectQuery)query).Fields.Count);
            Assert.AreEqual(3, ((SelectQuery)query).Wheres.Count);
            Assert.AreEqual("MyField", ((SelectQuery)query).Wheres[0].Field);
            Assert.AreEqual("True", ((SelectQuery)query).Wheres[0].Value);
            Assert.AreEqual("OtherField", ((SelectQuery)query).Wheres[1].Field);
            Assert.AreEqual("False", ((SelectQuery)query).Wheres[1].Value);
            Assert.AreEqual("finalField", ((SelectQuery)query).Wheres[2].Field);
            Assert.AreEqual("nothing", ((SelectQuery)query).Wheres[2].Value);
        }

        [Test]
        public void SelectJoin()
        {
            string stmt = "SELECT * FROM MyTable JOIN OtherTable ON MyField = NewField WHERE MyField = 'True' AND OtherField = 'False' AND finalField='nothing'";
            IQuery query = qb.Parse(stmt);

            Assert.AreEqual(typeof(SelectQuery), query.GetType());
            Assert.AreEqual("MyTable", query.TableName);
            Assert.AreEqual(true, ((SelectQuery)query).SelectAll);
            Assert.AreEqual(0, ((SelectQuery)query).Fields.Count);
            Assert.AreEqual(3, ((SelectQuery)query).Wheres.Count);
            Assert.AreEqual("MyField", ((SelectQuery)query).Wheres[0].Field);
            Assert.AreEqual("True", ((SelectQuery)query).Wheres[0].Value);
            Assert.AreEqual("OtherField", ((SelectQuery)query).Wheres[1].Field);
            Assert.AreEqual("False", ((SelectQuery)query).Wheres[1].Value);
            Assert.AreEqual("finalField", ((SelectQuery)query).Wheres[2].Field);
            Assert.AreEqual("nothing", ((SelectQuery)query).Wheres[2].Value);

            Assert.AreEqual(1, ((SelectQuery)query).Joins.Count);
            Assert.AreEqual("OtherTable", ((SelectQuery)query).Joins[0].TableName);
            Assert.AreEqual("MyField", ((SelectQuery)query).Joins[0].LeftField);
            Assert.AreEqual("NewField", ((SelectQuery)query).Joins[0].RightField);
        }

        [Test]
        public void SelectMultipleJoin()
        {
            string stmt = "SELECT * FROM MyTable " +
                          "JOIN OtherTable ON MyField = NewField " +
                          "JOIN Other2Table ON MyField = NewField2" + 
                          "WHERE MyField = 'True' " +
                          "AND OtherField = 'False' " +
                          "AND finalField='nothing'";

            IQuery query = qb.Parse(stmt);

            Assert.AreEqual(typeof(SelectQuery), query.GetType());
            Assert.AreEqual("MyTable", query.TableName);
            Assert.AreEqual(true, ((SelectQuery)query).SelectAll);
            Assert.AreEqual(0, ((SelectQuery)query).Fields.Count);
            Assert.AreEqual(3, ((SelectQuery)query).Wheres.Count);
            Assert.AreEqual("MyField", ((SelectQuery)query).Wheres[0].Field);
            Assert.AreEqual("True", ((SelectQuery)query).Wheres[0].Value);
            Assert.AreEqual("OtherField", ((SelectQuery)query).Wheres[1].Field);
            Assert.AreEqual("False", ((SelectQuery)query).Wheres[1].Value);
            Assert.AreEqual("finalField", ((SelectQuery)query).Wheres[2].Field);
            Assert.AreEqual("nothing", ((SelectQuery)query).Wheres[2].Value);

            Assert.AreEqual(2, ((SelectQuery)query).Joins.Count);
            Assert.AreEqual("OtherTable", ((SelectQuery)query).Joins[0].TableName);
            Assert.AreEqual("MyField", ((SelectQuery)query).Joins[0].LeftField);
            Assert.AreEqual("NewField", ((SelectQuery)query).Joins[0].RightField);
            Assert.AreEqual("Other2Table", ((SelectQuery)query).Joins[0].TableName);
            Assert.AreEqual("MyField", ((SelectQuery)query).Joins[0].LeftField);
            Assert.AreEqual("NewField2", ((SelectQuery)query).Joins[0].RightField);
        }

        [Test]
        public void SelectMultipleDifferentJoins()
        {
            string stmt = "SELECT * FROM MyTable " +
                          "LEFT OUTER JOIN OtherTable ON MyField = NewField " +
                          "RIGHT INNER JOIN Other2Table ON MyField = NewField2" +
                          "WHERE MyField = 'True' " +
                          "AND OtherField = 'False' " +
                          "AND finalField='nothing'";

            IQuery query = qb.Parse(stmt);

            Assert.AreEqual(typeof(SelectQuery), query.GetType());
            Assert.AreEqual("MyTable", query.TableName);
            Assert.AreEqual(true, ((SelectQuery)query).SelectAll);
            Assert.AreEqual(0, ((SelectQuery)query).Fields.Count);
            Assert.AreEqual(3, ((SelectQuery)query).Wheres.Count);
            Assert.AreEqual("MyField", ((SelectQuery)query).Wheres[0].Field);
            Assert.AreEqual("True", ((SelectQuery)query).Wheres[0].Value);
            Assert.AreEqual("OtherField", ((SelectQuery)query).Wheres[1].Field);
            Assert.AreEqual("False", ((SelectQuery)query).Wheres[1].Value);
            Assert.AreEqual("finalField", ((SelectQuery)query).Wheres[2].Field);
            Assert.AreEqual("nothing", ((SelectQuery)query).Wheres[2].Value);

            Assert.AreEqual(2, ((SelectQuery)query).Joins.Count);
            Assert.AreEqual("OtherTable", ((SelectQuery)query).Joins[0].TableName);
            Assert.AreEqual("MyField", ((SelectQuery)query).Joins[0].LeftField);
            Assert.AreEqual("NewField", ((SelectQuery)query).Joins[0].RightField);
            Assert.AreEqual("Other2Table", ((SelectQuery)query).Joins[0].TableName);
            Assert.AreEqual("MyField", ((SelectQuery)query).Joins[0].LeftField);
            Assert.AreEqual("NewField2", ((SelectQuery)query).Joins[0].RightField);
        }
    }
}
