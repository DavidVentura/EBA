using System;
using System.Data;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace EBA
{
    internal partial class TableForm : Form
    {
        internal MySqlDataAdapter mySqlDataAdapter;
        internal bool tableChanged = false;
        int curTextLength = 2;
        internal TableForm()
        {
            InitializeComponent();
            dataGridView1.AutoSize = true;
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.DisplayedCells;
            Bind();
        }
        internal virtual void Bind()
        {
            throw new NotImplementedException();
        }

        internal DataRowCollection Rows
        {
            get { return ((DataTable)dataGridView1.DataSource).Rows; }
        }

        internal bool Save()
        {
            if (tableChanged)
            {
                if (minimumDataComplete())
                    try
                    {
                        mySqlDataAdapter.Update((DataTable)dataGridView1.DataSource);
                        tableChanged = false;
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show("Hubo un error con los datos\n" + e.Message);
                        return false;
                    }
                else
                {
                    MessageBox.Show("Complete el campo seleccionado");
                    return false;
                }
                MessageBox.Show("Datos guardados", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            return true;
        }

        internal virtual bool minimumDataComplete() { return false; }

        private void button1_Click(object sender, EventArgs e)
        {
            Save();
        }
        private void TableForm_Load(object sender, EventArgs e)
        {
            dataGridView1.MultiSelect = false;
            CenterToScreen();
        }

        internal virtual void dataGridView1_userAddedRow(object sender, System.Windows.Forms.DataGridViewRowEventArgs e)
        {
            tableChanged = true;
            dataGridView1.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCellsExceptHeader);
        }

        internal virtual void dataGridView1_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e)
        {
            if (MessageBox.Show("Seguro que quiere eliminar este registro?", "Atencion", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
            {
                tableChanged = true;
                e.Cancel = false;
                return;
            }
            e.Cancel = true;
        }

        internal virtual void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex>0)
                tableChanged = true;
        }
        private void button2_Click(object sender, EventArgs e)
        {
            if (tableChanged)
            {
                switch (MessageBox.Show("Hay modificaciones no guardadas.\nDesea guardar?", "Atencion", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question))
                {
                    case DialogResult.Yes:
                        Save();
                        break;
                    case DialogResult.Cancel:
                        return;
                }
            }
            Close();
        }

        private void textBox1_KeyUp(object sender, KeyEventArgs e)
        {
            if (textBox1.TextLength > 2 || textBox1.TextLength < curTextLength)
            {
                curTextLength = textBox1.TextLength;
                filter(textBox1.Text);
            }
        }

        internal virtual void filter(string p)
        {
            throw new NotImplementedException();
        }


    }
}
