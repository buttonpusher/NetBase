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
using System.IO;

namespace NetBase.Sql
{
    // <summary>
    // Does quite a lot of hard work to parse the query. Based on
    // Jack Crenshaw's "Let's Build a compiler". I know, should really
    // be using a parser generator, but the point is to learn how
    // these kind of things work.
    // </summary>
    internal class Tokenizer
    {
        private StringReader _sr;
        private char Look;
        private bool EOF;
        public string Token {get;set;}

        public Tokenizer(string stmt)
        {
            this._sr = new StringReader(stmt);
            // initialise by filling the char buffer
            GetChar();
            GetToken();
        }
        
        private void GetChar()
        {
            char[] buff = new char[1];
            // Yes, performance is not a major concern here.
            int EOF = _sr.Read(buff, 0, 1);

            if (EOF == 0)
            {
                this.EOF = true;
            }
            Look = buff[0];
        }

        private void ChewSpace()
        {
            while (Look == ' ')
            {
                GetChar();
            }
        }

        public void GetToken()
        {
            StringBuilder token = new StringBuilder();
            ChewSpace();

            // @TODO refactor this
            if (Look == '\'')
            {
                this.Token = GetQuotedToken();
                return;
            }

            if (Look == ',')
            {
                this.Token = ",";
                GetChar();
                return;
            }

            if (Look == '=')
            {
                this.Token = "=";
                GetChar();
                return;
            }

            if (Look == '(')
            {
                this.Token = "(";
                GetChar();
                return;
            }

            if (Look == ')')
            {
                this.Token = ")";
                GetChar();
                return;
            }

            while (Look != ' ' && Look != ',' && Look != '=' && Look != '(' && Look != ')' && !this.EOF)
            {
                token.Append(Look);
                GetChar();
            }
            this.Token = token.ToString();
        }

        private string GetQuotedToken()
        {
            StringBuilder token = new StringBuilder();
            ChewSpace();
            MatchChar('\'');
            while (Look != '\'' && !this.EOF)
            {
                token.Append(Look);
                GetChar();
            }
            MatchChar('\'');
            return token.ToString();
        }

        public void Match(string token)
        {
            if (this.Token != token)
            {
                throw new QuerySyntaxException("Syntax error: " + token + " expected");
            }
            GetToken();
        }
        
        private void MatchChar(char token)
        {
            ChewSpace();
            if (Look != token)
            {
                throw new QuerySyntaxException("Syntax error: " + token + " expected");
            }
            GetChar();
        }
    }
}
