using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace EBA
{
    internal partial class CuentaCorriente : Form
    {
        private int id;
        private tipoCliente tipoCliente;
        private MySqlDataReader r;
        private MySqlCommand cmd = Connection.CreateCommand();


        internal CuentaCorriente(string p, tipoCliente tipoCliente)
        {//todo fix
            InitializeComponent();
            id = Int32.Parse(p);
            this.tipoCliente = tipoCliente;
            //cmd.Connection = Connection.getConnection();
            string table = (tipoCliente == EBA.tipoCliente.CLIENTE ? "cccliente" : "ccdistribuidor");
            cmd.CommandText = "Select * from " + table + " where id=" + id;
            r = cmd.ExecuteReader();
            dataGridView1.ColumnCount = 5;
            dataGridView1.RowCount = 1;
            dataGridView1.Columns[0].HeaderText = "N° Factura";
            dataGridView1.Columns[1].HeaderText = "Fecha";
            dataGridView1.Columns[2].HeaderText = "Importe";
            dataGridView1.Columns[3].HeaderText = "A cuenta";
            dataGridView1.Columns[4].HeaderText = "Saldo";
            while (r.Read())
                dataGridView1.Rows.Add(r.GetInt16(0), r.GetDateTime(1), r.GetDecimal(2), r.GetDecimal(3), r.GetDecimal(4));
            
            r.Close();
            CenterToScreen();
        }

        private void CuentaCorriente_Load(object sender, EventArgs e)
        {

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}
