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
    internal partial class CuentasCorrientes : Form
    {
        MySqlDataReader r;
        MySqlCommand cmd = Connection.CreateCommand();
        internal CuentasCorrientes()
        {
            InitializeComponent();
        }

        private void CuentasCorrientes_Load(object sender, EventArgs e)
        {
            //todo: fix
            cmd.CommandText = "Select id, apellido, nombre,cuit,iva from clientes where id in (select id from cccliente)";
            r = cmd.ExecuteReader();
            dgClientes.ColumnCount = 5;
            dgClientes.RowCount = 0;
            dgClientes.Columns[0].HeaderText = "ID";
            dgClientes.Columns[1].HeaderText = "Apellido";
            dgClientes.Columns[2].HeaderText = "Nombre";
            dgClientes.Columns[3].HeaderText = "Cuit";
            dgClientes.Columns[4].HeaderText = "IVA";
            while (r.Read())
                dgClientes.Rows.Add(r.GetInt32(0), r.GetString(1), r.GetString(2), r.GetString(3), r.GetString(4));
            
            r.Close();
            cmd.CommandText = "Select id, rs,cuit,iva from distribuidores where id in (select id from ccdistribuidor)";
            r = cmd.ExecuteReader();
            dgDistribuidores.ColumnCount = 4;
            dgDistribuidores.RowCount = 0;
            dgDistribuidores.Columns[0].HeaderText = "ID";
            dgDistribuidores.Columns[1].HeaderText = "Razon social";
            dgDistribuidores.Columns[2].HeaderText = "Cuit";
            dgDistribuidores.Columns[3].HeaderText = "IVA";
            while (r.Read())
                dgDistribuidores.Rows.Add(r.GetInt32(0), r.GetString(1), r.GetString(2), r.GetString(3));
            r.Close();
            CenterToScreen();
        }

        void dgDistribuidores_CellDoubleClick(object sender, System.Windows.Forms.DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return; //header
            Form f = new CuentaCorriente(dgDistribuidores.Rows[e.RowIndex].Cells[0].Value.ToString(), tipoCliente.DISTRIBUIDOR);
            f.Show();
        }

        void dgClientes_CellDoubleClick(object sender, System.Windows.Forms.DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return; //header
            Form f = new CuentaCorriente(dgClientes.Rows[e.RowIndex].Cells[0].Value.ToString(), tipoCliente.CLIENTE);
            f.Show();
        }
    }
}
