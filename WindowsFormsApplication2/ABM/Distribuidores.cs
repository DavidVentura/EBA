using System;
using MySql.Data.MySqlClient;
namespace EBA
{
    class Distribuidores : TableForm
    {
        internal Distribuidores()
        {
            this.Text = "Distribuidores";
            dataGridView1.Columns[0].ReadOnly = true;
            dataGridView1.Columns[0].Visible = false;
            dataGridView1.Columns[2].HeaderText = "Razón social";
        }

        internal override bool minimumDataComplete()
        {
            for (int i = 0; i < dataGridView1.RowCount - 1; i++) //-1 por la vacia
            {
                for (int j = 0; j < 3; j++) //columnas
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
            // Distribuidores
            // 
            this.ClientSize = new System.Drawing.Size(292, 272);
            this.Name = "Distribuidores";
            this.Load += new System.EventHandler(this.Distribuidores_Load);
            this.ResumeLayout(false);

        }

        private void Distribuidores_Load(object sender, EventArgs e)
        {

        }

        internal override void dataGridView1_userAddedRow(object sender, System.Windows.Forms.DataGridViewRowEventArgs e)
        {
            if (dataGridView1.Rows.Count == 2)
            {
                if (dataGridView1.Rows[0].Cells[0] == null || dataGridView1.Rows[0].Cells[0].Value.ToString().Length == 0)
                dataGridView1.Rows[0].Cells[0].Value = 1;
                return;
            }

            if (dataGridView1.Rows[dataGridView1.Rows.Count - 1].Cells[0].Value == null || dataGridView1.Rows[dataGridView1.Rows.Count - 1].Cells[0].Value.ToString().Length == 0)
            {
                dataGridView1.Rows[dataGridView1.Rows.Count - 2].Cells[0].Value = (int)(dataGridView1.Rows[dataGridView1.Rows.Count - 3].Cells[0].Value) + 1;
            }
        }
        internal override void filter(string p)
        {
            dataGridView1.DataSource = Queries.getDistribuidoresDataTable(out mySqlDataAdapter, p);
        }
        internal override void Bind()
        {
            dataGridView1.DataSource = Queries.getDistribuidoresDataTable(out mySqlDataAdapter, "");
        }
    }
}
