using System;
using MySql.Data.MySqlClient;

namespace EBA
{
    class Productos : TableForm
    {
        internal Productos()
        {
            this.Text = "Productos";
        }

        internal override bool minimumDataComplete()
        {
            for (int i = 0; i < dataGridView1.RowCount - 1; i++) //-1 por la vacia
            {
                for (int j = 0; j < 3; j++) //3 columnas
                {
                    if (dataGridView1.Rows[i].Cells[j].Value == null || dataGridView1.Rows[i].Cells[j].Value.ToString().Length == 0)
                    {
                        dataGridView1.CurrentCell = dataGridView1.Rows[i].Cells[j];
                        return false;
                    }
                }
            }
            return true;
        }


        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // Productos
            // 
            this.ClientSize = new System.Drawing.Size(292, 272);
            this.Name = "Productos";
        //    this.Load += new System.EventHandler(this.Productos_Load);
            this.ResumeLayout(false);

        }
        internal override void filter(string p)
        {
            dataGridView1.DataSource = Queries.getProductosDataTable(out mySqlDataAdapter, p);
        }
        internal override void Bind()
        {
            dataGridView1.DataSource = Queries.getProductosDataTable(out mySqlDataAdapter, "");
        }

    }
}
