using System;
using System.Windows.Forms;
namespace EBA
{
    internal partial class MainForm : Form
    {
        
        internal MainForm()
        {
            InitializeComponent();
            Props.Initialize();
            Connection.Initialize();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            CenterToScreen();
        }

        private void Clientes_Click(object sender, EventArgs e)
        {
            TableForm c = new Clientes();
            c.Show();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            TableForm p = new Productos();
            p.Show();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            TableForm p = new Distribuidores();
            p.Show();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            TableForm p = new Insumos();
            p.Show();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            new InsumosProductos().Show();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Form f = new Facturacion();
            f.Show();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            TableForm p = new Proveedores();
            p.Show();
        }

        private void button8_Click(object sender, EventArgs e)
        {
           // FKForm f = new InsumosProveedores();
           // f.Show();
        }

        private void button9_Click(object sender, EventArgs e)
        {
            Form f = new CuentasCorrientes();
            f.Show();
        }

        /*private void button10_Click(object sender, EventArgs e)
        {
            Consultas.ConsultaDistribuidores c = new Consultas.ConsultaDistribuidores();
            c.Show();
        }*/
    }
}
