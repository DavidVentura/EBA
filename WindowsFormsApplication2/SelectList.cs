using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace EBA
{
    internal partial  class SelectList : Form
    {
        MySqlDataAdapter mySqlDataAdapter;
        DataTable dataTable;
        BindingSource bindingSource;
        String table;
        internal int returnValue = -1;
        internal SelectList(String t)
        {
            table = t;
            
            InitializeComponent();
            dataGridView1.AutoSize = true;
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.DisplayedCells;
            Bind();
        }

        internal SelectList()
        {
    }

        internal void Bind()
        {
            string query = "SELECT * FROM " + table;

            mySqlDataAdapter = new MySqlDataAdapter(query, Connection.getConnection());
            
            dataTable = new DataTable();
            mySqlDataAdapter.Fill(dataTable);
            bindingSource = new BindingSource();
            bindingSource.DataSource = dataTable;

            dataGridView1.DataSource = bindingSource;
            int totWidth = 500;
            dataGridView1.ReadOnly = true;            
            if (this.Width < totWidth){
                this.Width = totWidth;
            }
        }

        internal DataRowCollection Rows
        {
            get { return dataTable.Rows; }
        }



        internal virtual bool minimumDataComplete()
        {
            return false;
        }


        private void button1_Click(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentCell == null)
            {
                returnValue = -1;
                this.Close();
                return;
            }
            returnValue = Int32.Parse(dataGridView1.Rows[dataGridView1.CurrentCell.RowIndex].Cells[0].Value.ToString());
            this.Close();
        }
        private void TableForm_Load(object sender, EventArgs e)
        {
            this.MinimumSize = new Size(600, 300);
            dataGridView1.MultiSelect = false;
        }
        private void TableForm_OnResize(object sender,  EventArgs e)
        {
            dataGridView1.AutoResizeColumns();
            dataGridView1.Width = this.Width - 30;
            dataGridView1.Height = this.Height - 80;
            button1.Top = dataGridView1.Bottom + 5;
            
        }


        private void datagridview1_keyup(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
                this.Dispose();
            if (e.KeyCode == Keys.Enter)
                button1_Click(null, null);
        }

    }
}
