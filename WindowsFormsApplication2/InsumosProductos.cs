using System;
using System.Windows.Forms;

namespace EBA
{
    public partial class InsumosProductos : Form
    {
        public InsumosProductos()
        {
            InitializeComponent();
        }
        int codigo = 0;

        private void InsumosProductos_Load(object sender, EventArgs e)
        {
            SelectList s = new SelectList("productos");
            s.ShowDialog();
            codigo = s.returnValue;
            if (codigo == -1) this.Close();
            treeView1.CheckBoxes = true;
            treeView1.Nodes.AddRange(Queries.getInsumos(codigo));
            treeView1.ExpandAll();
        }
    }
}
