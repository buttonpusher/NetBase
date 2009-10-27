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
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.Common;

namespace View
{
    public partial class Form1 : Form
    {
        private DbConnection db;

        public Form1()
        {
            InitializeComponent();
            Init();
        }

        public void Init()
        {
            this.db = new NetBase.Api.DbfConnection();
            // set database location to the current directory
            this.db.ConnectionString = ".";
            
        }

        public void DisplayRows(DbDataReader nt)
        {
            this.listView1.Clear();

            for (int i = 0; i < nt.FieldCount; i++)
            {
                listView1.Columns.Add(nt.GetName(i));
            }
            int rowCount = 0;
            while (nt.NextResult())
            {
                rowCount++;
                string contents = "";
                
                if (nt[0] != null)
                {
                    contents = nt[0].ToString();
                }

                ListViewItem lvi = new ListViewItem(contents);
                for (int i = 1; i < nt.FieldCount; i++)
                {
                    contents = "";
                    if (nt[i] != null)
                    {
                        contents = nt[i].ToString();
                    }
                    lvi.SubItems.Add(contents);
                }
                this.listView1.Items.Add(lvi);
            }
            this.toolStripStatusLabel1.Text = rowCount.ToString() + " row(s) returned.";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                DbCommand cmd = db.CreateCommand();
                cmd.CommandText = this.textBox1.Text;
                DbDataReader dbr = cmd.ExecuteReader();
                if (dbr == null)
                {
                    MessageBox.Show("No results returned");
                    listView1.Clear();
                    return;
                }
                DisplayRows(dbr);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

    }
}
