using System;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using System.Collections.Generic;


namespace EBA.Consultas
{
    public partial class ConsultaDistribuidores : Form
    {
        

        public ConsultaDistribuidores()
        {
            InitializeComponent();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (filter.TextLength > 3)
            {
                List<Distribuidor> l = Queries.getDistribuidores(filter.Text);
                dataGridView1.ColumnCount = 2;
                
            }
        }

        private void ConsultaDistribuidores_Load(object sender, EventArgs e)
        {

        }
    }
}
