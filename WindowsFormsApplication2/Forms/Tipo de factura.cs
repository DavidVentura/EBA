using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace EBA
{
    internal partial class Tipo_de_factura : Form
    {
        private tipoFactura t = tipoFactura.INVALID;
        internal int porcentajeFactura=0;
        internal int porcentajePPTO = 100;
        internal Tipo_de_factura(tipoIVA t)
        {
            InitializeComponent();
            if (t == tipoIVA.RESPONSABLE_INSCRIPTO)
            {
                radioButton2.Visible = true;
                radioButton3.Visible = false;
            }
            else
            {
                radioButton3.Visible = true;
                radioButton3.Left = radioButton2.Left;
                radioButton2.Visible = false;
            }
        }

        private void Tipo_de_factura_Load(object sender, EventArgs e)
        {
            Height = 120;
            label1.Visible = false;
            label2.Visible = false;
            textBox1.Visible = false;
            textBox2.Visible = false;
            radioButton1.Checked = true;
            CenterToScreen();
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton2.Checked)
            {
                Height = 170;
                label1.Visible = true;
                label2.Visible = true;
                textBox1.Visible = true;
                textBox2.Visible = true;
            }
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton1.Checked)
            {
                Height = 120;
                label1.Visible = false;
                label2.Visible = false;
                textBox1.Visible = false;
                textBox2.Visible = false;
            }
        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton3.Checked)
            {
                Height = 170;
                label1.Visible = true;
                label2.Visible = true;
                textBox1.Visible = true;
                textBox2.Visible = true;
            }
        }
        internal tipoFactura value()
        {
            return t;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (radioButton1.Checked) t =  tipoFactura.PRESUPUESTO;
            if (radioButton2.Checked) t = tipoFactura.FACTURA_A;
            if (radioButton3.Checked) t = tipoFactura.FACTURA_B;
            this.Close();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            int n =0;

            if (Int32.TryParse(textBox1.Text, out n))
            {
                if (n > 100)
                {
                    n = 100;
                    textBox1.Text = "100";
                }
                textBox2.Text = (100 - n).ToString();
            }
            else
                textBox1.Text = "0";

            porcentajeFactura = n;
            porcentajePPTO = 100 - n;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            t = tipoFactura.INVALID;
            Close();
        }
    }
}
