using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace EBA
{
    public partial class InsumosProductos : Form
    {

        public InsumosProductos()
        {
            InitializeComponent();
        }
        int codigo = 0;
        bool hasChanged = false;

        private void InsumosProductos_Load(object sender, EventArgs e)
        {
            SelectList s = new SelectList("productos","Elegi el producto");
            s.ShowDialog();
            codigo = s.returnValue;
            if (codigo == -1)
            {
                this.Close();
                return;
            }
            treeView1.CheckBoxes = true;
            treeView1.Nodes.AddRange(Queries.getInsumos(codigo));
            treeView1.ExpandAll();
            Text = "Selecciona los insumos del producto " + codigo;
            CenterToScreen();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            List<int> ids = new List<int>();
            foreach (TreeNode tn in treeView1.Nodes)
            {
                foreach (TreeNode n in tn.Nodes)
                {
                    if (n.Checked && n.Tag.ToString().Length > 0)
                        ids.Add(Convert.ToInt16(n.Tag));
                }
            }
            Queries.setInsumos(codigo, ids.ToArray());
            hasChanged = false;
        }

        private void treeView1_BeforeCheck(object sender, TreeViewCancelEventArgs e) 
        {
            if (e.Node.Level == 0)
            {
                e.Cancel = true;
                return;
            }
            hasChanged = true;
        }
        private void button2_Click(object sender, EventArgs e)
        {
            if (hasChanged)
            {
                if (MessageBox.Show("Salir sin guardar?", "Advertencia", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
                    Close();
            }
            else
                Close();
        }

    }
}
