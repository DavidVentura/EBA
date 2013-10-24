using System;
using System.Collections.Generic;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace EBA.Forms
{
    public partial class ListaClientes : Form
    {
        internal Cliente c = null;
        List<Cliente> clientList = new List<Cliente>();
        MySqlDataReader r;
        MySqlCommand cmd = Connection.CreateCommand();
        public ListaClientes()
        {
            InitializeComponent();
            dataGridView1.ColumnCount = 5;
            dataGridView1.Columns[0].Visible = false;
            dataGridView1.Columns[1].HeaderText = "Nombre";
            dataGridView1.Columns[2].HeaderText = "Razon social";
            dataGridView1.Columns[3].HeaderText = "Direccion";
            dataGridView1.Columns[4].HeaderText = "Telefono";
        }

        private void ListaClientes_Load(object sender, EventArgs e)
        {
            getClients("");
            CenterToScreen();
        }

        private void getClients(string p)
        {
            dataGridView1.Rows.Clear(); 
            if (p.Trim().Length>0)
                clientList = Queries.getClients(p);
            else
                clientList = Queries.getClients();
            foreach (Cliente c in clientList)
                dataGridView1.Rows.Add(c.id, c.getNombre(), c.getRazonSocial(), c.direccion, c.telefono);

            if (dataGridView1.Rows.Count > 0)
            {
                if (dataGridView1.SelectedRows.Count == 0)
                    dataGridView1.Rows[0].Selected = true;
                button1.Enabled = true;
            } 
            else 
                button1.Enabled = false;
        }

        private void textBox1_KeyUp(object sender, KeyEventArgs e)
        {

            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                button1_Click(sender, e);

            }
            else
            {
                if (textBox1.TextLength > 1 || e.KeyCode == Keys.Delete || e.KeyCode == Keys.Back)
                {
                    getClients(textBox1.Text);
                }
            }
        }


        private void button1_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < clientList.Count; i++)
                if (clientList[i].id == Int32.Parse(dataGridView1.SelectedRows[0].Cells[0].Value.ToString()) &&
                    clientList[i].getNombre() == dataGridView1.SelectedRows[0].Cells[1].Value.ToString())
                {
                    c = clientList[i];
                    break;
                }
            Dispose();
        }

        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            button1_Click(sender, e);
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }
        
    }
}
