using System;
using System.Data;
using System.Drawing;
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

        internal SelectList(String tabla, string titulo) : this(tabla)
        {
            Text = titulo;
        }
        internal SelectList()
        {
    }

        internal void Bind()
        {
            dataGridView1.DataSource = Queries.getTable(table, "", out mySqlDataAdapter);
            int totWidth = 500;
            dataGridView1.ReadOnly = true;            
            if (this.Width < totWidth)
                this.Width = totWidth;
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
            this.MinimumSize = new Size(500, 500);
            dataGridView1.MultiSelect = false;
            CenterToScreen();
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
