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
    // This class responsible for parsing the SQL statement and
    // creating a query object. Why haven't we used a parser generator...
    // well, how would we ever learn anything?
    // However, this is a very large class and could be broken down a little.
    //
    // The entry point is really the "Expression" function at the bottom.
    // The key element here is really the tokenizer.
    //
    // </summary>
    internal class QueryBuilder
    {
        private Tokenizer tokenizer;
        private IQuery Query;
        
        private void FieldList()
        {
            if (tokenizer.Token == "*")
            {
                tokenizer.Match("*");
                ((SelectQuery)this.Query).SelectAll = true;
                return;
            }

            do
            {
                if (tokenizer.Token == ",") tokenizer.Match(",");
                ((SelectQuery)this.Query).Fields.Add(tokenizer.Token);
                tokenizer.GetToken();
            } while (tokenizer.Token == ",");
        }

        private void JoinClause()
        {
            // Large set of defaults for joins.
            // This is where we start to think about moving some
            // parsing into the individual elements, esp. since
            // the responsibility for executing the query is
            // delegated to the executive.
            do
            {
                Join j = null;
                Join.OrientationType orientation = Join.OrientationType.Left;

                if (tokenizer.Token == "LEFT")
                {
                    orientation = Join.OrientationType.Left;
                    tokenizer.Match("LEFT");
                }

                if (tokenizer.Token == "RIGHT")
                {
                    orientation = Join.OrientationType.Right;
                    tokenizer.Match("RIGHT");
                }

                if (tokenizer.Token == "JOIN")
                {
                    tokenizer.Match("JOIN");
                    j = new InnerJoin();
                }

                if (tokenizer.Token == "INNER")
                {
                    tokenizer.Match("INNER");
                    tokenizer.Match("JOIN");
                    j = new InnerJoin();
                }

                if (tokenizer.Token == "OUTER")
                {
                    tokenizer.Match("OUTER");
                    tokenizer.Match("JOIN");
                    j = new OuterJoin();
                }

                j.Orientation = orientation;
                j.TableName = tokenizer.Token;
                tokenizer.GetToken();

                tokenizer.Match("ON");
                j.LeftField = tokenizer.Token;
                tokenizer.GetToken();
                tokenizer.Match("=");
                j.RightField = tokenizer.Token;
                tokenizer.GetToken();
                ((SelectQuery)this.Query).Joins.Add(j);
            } while (tokenizer.Token == "JOIN" ||
                    tokenizer.Token == "LEFT" ||
                    tokenizer.Token == "RIGHT" ||
                    tokenizer.Token == "INNER" ||
                    tokenizer.Token == "OUTER");

        }

        private void WhereClause()
        {
            tokenizer.Match("WHERE");
            
            do
            {
                if (tokenizer.Token == "AND") tokenizer.Match("AND");

                string Field = tokenizer.Token;
                tokenizer.GetToken();
                tokenizer.Match("=");
                string value = tokenizer.Token;
                Where w = new Where();
                w.Field = Field;
                w.Value = value;
                ((SelectQuery)this.Query).Wheres.Add(w);
                tokenizer.GetToken();
            } while (tokenizer.Token == "AND");
        }

        private void ValueList()
        {
            tokenizer.Match("VALUES");
            tokenizer.Match("(");

            for (int i = 0; i < ((InsertQuery)this.Query).Sets.Count; i++)
            {
                if (tokenizer.Token == ",") tokenizer.Match(",");

                string value = tokenizer.Token;
                tokenizer.GetToken();

                ((InsertQuery)this.Query).Sets[i].Value = value;
            } 

            tokenizer.Match(")");
        }

        private void BracketedFieldList()
        {
            tokenizer.Match("(");
            do
            {
                if (tokenizer.Token == ",") tokenizer.Match(",");

                string Field = tokenizer.Token;
                tokenizer.GetToken();
                Set w = new Set();
                w.Field = Field;
                
                ((InsertQuery)this.Query).Sets.Add(w);
                
            } while (tokenizer.Token == ",");

            tokenizer.Match(")");
        }

        private void SelectExpression()
        {
            tokenizer.Match("SELECT");
            Query = new SelectQuery();

            FieldList();

            tokenizer.Match("FROM");

            Query.TableName = tokenizer.Token;
            tokenizer.GetToken();


            if (tokenizer.Token == "JOIN")
            {
                JoinClause();
            }

            if (tokenizer.Token == "WHERE")
            {
                WhereClause();
            }

        }

        private void InsertExpression()
        {
            tokenizer.Match("INSERT");
            Query = new InsertQuery();
            tokenizer.Match("INTO");

            Query.TableName = tokenizer.Token;
            tokenizer.GetToken();
            
            BracketedFieldList();
            ValueList();
        }

        private void CreateExpression()
        {
            tokenizer.Match("CREATE");
            tokenizer.Match("TABLE");
            Query = new CreateTableQuery();
            Query.TableName = tokenizer.Token;
            tokenizer.GetToken();
            tokenizer.Match("(");
            do
            {
                CreateTableQuery.CreateField f = new CreateTableQuery.CreateField();
                f.Name = tokenizer.Token;
                ((CreateTableQuery)Query).Fields.Add(f);
                tokenizer.GetToken();

                if (tokenizer.Token == ",")
                {
                    tokenizer.Match(",");
                }

            } while (tokenizer.Token != ")");

            tokenizer.Match(")");
        }

        private void Expression()
        {
            if (tokenizer.Token == "SELECT")
            {
                SelectExpression();
            }
            else if (tokenizer.Token == "INSERT")
            {
                InsertExpression();
            }
            else if (tokenizer.Token == "CREATE")
            {
                CreateExpression();
            }
            else
            {
                throw new QuerySyntaxException("Syntax Error: Expected \"select\"; \"insert\"; or \"create table\" clause");
            }
        }

        public IQuery Parse(string stmt)
        {
            this.tokenizer = new Tokenizer(stmt);
            Expression();
            return this.Query;
        }
    }
}
