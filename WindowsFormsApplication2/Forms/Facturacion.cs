using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Collections;


namespace EBA
{
    internal partial class Facturacion : Form
    {
        Factura factura = new Factura();
        const int HEADER_LENGTH = 6;
        //Cliente curClient;
        AutoCompleteStringCollection productCodeList = new AutoCompleteStringCollection();
        AutoCompleteStringCollection productNameList = new AutoCompleteStringCollection();

        DataGridViewComboBoxCell formaDePagoC = new DataGridViewComboBoxCell();
        private bool validKey = true;
        int subtotalRow=2;
        StringFormat strFormat; //Used to format the grid rows.
        ArrayList arrColumnLefts = new ArrayList();//Used to save left coordinates of columns
        ArrayList arrColumnWidths = new ArrayList();//Used to save column widths
        int iCellHeight = 0; //Used to get/set the datagridview cell height
        int iTotalWidth = 0; //
        int iRow = 0;//Used as counter
        bool bFirstPage = false; //Used to check whether we are printing first page
        const int rowCount = 33;
        int iHeaderHeight = 0; //Used for the header height
        Factura FacturaImpresa;
        internal Facturacion()
        {
            InitializeComponent();
        }

        private void Facturacion_Load(object sender, EventArgs e)
        {
            
            List<String> formaDePago = new List<String>();
            formaDePago.Add("Contado");
            formaDePago.Add("Cuenta corriente");
            formaDePago.Add("Otra");

            dg.RowCount = 35;
            dg.AllowUserToAddRows = false;
            dg.ColumnCount = 5;
            dg.ColumnHeadersVisible = false;
            dg.RowHeadersVisible = false;
            dg.AllowUserToResizeRows = false;
            dg.AllowUserToResizeColumns = false;
            
            formaDePagoC.Value = formaDePago[0];
            formaDePagoC.DataSource = formaDePago;
            formaDePagoC.DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing;
            dg[4, 3] = formaDePagoC;


            setHeader();
            setFooter();
            subtotalRow =2;
            /*foreach (Cliente c in factura.clientList)
                clientList.Add(c.getNombre());*/
            foreach (Producto p in factura.productsList)
            {
                productCodeList.Add(p.codigo.ToString());
                productNameList.Add(p.nombre);
            }

            dg.MultiSelect = false;
            dg.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
            //dg.CellBorderStyle = DataGridViewCellBorderStyle.None; //deberia dar performance pero nope
            
            //todo setear ancho a mano
            CenterToScreen();
        }

        private void setHeader()
        {
            dg[1, 0].Value = "Señor/es:";
            dg[1, 0].ReadOnly = true;
            dg[1, 1].Value = "Domicilio:";
            dg[1, 1].ReadOnly = true;
            dg[1, 2].Value = "IVA:";
            dg[1, 2].ReadOnly = true;
            dg[1, 3].Value = "Remito Nº:";
            dg[1, 3].ReadOnly = true;
            dg[2, 3].Value = "S/D";
            //dg[1, 4].Value = "Tomado por:";
            dg[1, 4].ReadOnly = true;
            //dg[1, 5].Value = "ID:";
            dg[1, 5].ReadOnly = true;

            dg[3, 0].Value = "Fecha:";
            dg[3, 0].ReadOnly = true;
            dg[3, 1].Value = "Teléfono:";
            dg[3, 1].ReadOnly = true;
            dg[3, 2].Value = "CUIT:";
            dg[3, 2].ReadOnly = true;
            dg[3, 3].Value = "Forma de pago:";
            dg[3, 3].ReadOnly = true;
            dg[3, 4].Value = "Pedido Nº:";
            dg[3, 4].ReadOnly = true;

            dg[0, 6].Value = "Código";
            dg[0, 6].ReadOnly = true;
            dg[1, 6].Value = "Cantidad";
            dg[1, 6].ReadOnly = true;
            dg[2, 6].Value = "Descripción";
            dg[2, 6].ReadOnly = true;
            dg[3, 6].Value = "Precio unitario";
            dg[3, 6].ReadOnly = true;
            dg[4, 6].Value = "Precio final";
            dg[4, 6].ReadOnly = true;
            //dg[5, 0].Value = "ID";
            for (int i = 0; i < dg.ColumnCount; i++)
            {
                for (int j = 0; j < dg.RowCount; j++)
                    dg[i, j].ReadOnly = !isCellEditable(i, j);
            }
            dg.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dg.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dg.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dg.Columns[2].MinimumWidth = 300;
            dg.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dg.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

        }
        internal void dg_KeyPress(object sender, KeyEventArgs e)
        {

            if (!validKey)
                e.Handled = true;
        }
        internal void dg_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyValue == 46 && dg.CurrentRow.Index > HEADER_LENGTH)
            {
                factura.tryRemoveProduct(dg[0, dg.CurrentRow.Index].Value);
                updateProducts();
                updateFooter();
            }
            if (dg.CurrentCell.ColumnIndex <= 1)
            {
                if (e.KeyValue < 48 || e.KeyValue > 58)
                {
                    validKey = false;
                }
                validKey = true;
            }
        }

        internal void dg_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex == 0 && e.ColumnIndex == 2)
            {
                Forms.ListaClientes f = new Forms.ListaClientes();
                f.ShowDialog();
                if (f.c != null)
                {
                    factura.setClient(f.c);
                    dg[2, 0].Value = factura.cliente.getNombre();
                    dg[2, 1].Value = factura.cliente.direccion;
                    dg[2, 2].Value = factura.cliente.iva;
                    dg[4, 1].Value = factura.cliente.telefono;
                    dg[4, 0].Value = DateTime.Today.ToShortDateString();
                    setClientFooter();
                    updateFooter();
                    dg.CommitEdit(DataGridViewDataErrorContexts.Commit);
                }

            }
                
        }
        internal void dg_cellEndEditEventHandler(object sender, DataGridViewCellEventArgs e)
        {

            int curRow = e.RowIndex;
            if (e.RowIndex >= 32) return;
            if (e.RowIndex > HEADER_LENGTH) //si intento saltearse alguna fila...
            {
                for (int i = HEADER_LENGTH+1; i < e.RowIndex; i++)
                    if (dg[0, i].Value == null || dg[0,i].Value.ToString().Length==0) {
                        curRow = i;
                        break;
                    }
                int codigo=-1;
                if (dg[0, e.RowIndex].Value != null)
                {
                    if (dg[0, e.RowIndex].Value.ToString().Length > 0)
                    {
                        Int32.TryParse(dg[0, e.RowIndex].Value.ToString(), out codigo);
                    }
                }
                switch (e.ColumnIndex){
                    case 0:
                        if (!factura.tryAddProduct(codigo)) //fixme  lento
                            removeLine(e.RowIndex);
                        break;
                    case 1:
                        if (factura.productListed(codigo)) 
                        {
                            factura.trySetQuantity(codigo, dg[1, e.RowIndex].Value);
                                //dg.CurrentCell = dg[0, e.RowIndex + 1]; //no funciona
                            
                        }
                        break;
                    case 2:
                        string nombre="";
                        if (dg[2, e.RowIndex].Value != null)
                        {
                            if (dg[2, e.RowIndex].Value.ToString().Length > 0)
                                nombre = dg[2, e.RowIndex].Value.ToString();
                        }
                        if (!factura.tryAddProduct(nombre)) //fixme rapido
                        {
                            removeLine(e.RowIndex);
                        }
                        break;
                }
                updateProducts();
                updateFooter();
                return;
            }

            if (e.ColumnIndex == 4 && e.RowIndex == 4) //combobox
            {
                int result=0;
                if (dg[4, 4].Value == null)
                {
                    dg[4, 4].Value = 0;
                }
                if (Int32.TryParse(dg[4, 4].Value.ToString(),out result))
                    factura.numeroFactura = result;
                else
                {
                    dg[4, 4].Value = "";
                }
            }

            if (e.ColumnIndex == 4 && e.RowIndex == 3) //combobox
                factura.isCC = dg[4, 3].Value.ToString().Equals("Cuenta corriente");
            

        }
        private void updateProducts()
        {
            //fixme es lento esto .. en casa no?
            short n = 0;
            string[] emptyCols = new string[dg.ColumnCount];
            for (int i = HEADER_LENGTH + 1; i <= HEADER_LENGTH + factura.productList.Count + 1; i++)
            {
                if (dg.Rows[i].Cells[1].Value == null)
                {
                    dg.Rows[i].SetValues(emptyCols);
                    continue;
                }
                Int16.TryParse(dg.Rows[i].Cells[1].Value.ToString(), out n);
                if (!factura.productListed(n))
                {
                    dg.Rows[i].SetValues(emptyCols);
                    continue;
                }
                Int16.TryParse(dg.Rows[i].Cells[1].Value.ToString(), out n);
                if (n < 1)
                    dg.Rows[i].SetValues(emptyCols);
            }
            int row = HEADER_LENGTH;
            foreach (ProductoCantidad p in factura.productList)
            {
                row++;
                dg[0, row].Value = p.p.codigo;
                dg[1, row].Value = p.cantidad;
                dg[2, row].Value = p.p.nombre;
                dg[3, row].Value = p.p.precioIVA;
                dg[4, row].Value = p.p.precioIVA * p.cantidad;
            }

        }
        private void updateFooter()
        {

            Numalet n = new Numalet();

            dg[2, dg.RowCount - 3].Value = n.ToCustomCardinal(factura.getTotal());
            for (int i = factura.descuentos.Count-1; i >=0; i--)
                dg[4, dg.RowCount - (factura.descuentos.Count - i + 1)].Value = "$" + factura.getTotalDiscount(i); //+1 por el "Total" que va abajo.. -i para que vaya en el orden correcto
            
            dg[4, dg.RowCount - subtotalRow].Value = "$" + factura.getSubtotal();
            dg[4, dg.RowCount - 1].Value = "$" + factura.getTotal();
        }


        

        private void setClientFooter()
        {
            dg[2, 2].Value = factura.cliente.iva;
            dg[4, 2].Value = factura.cliente.cuit;
            setFooter();
        }
        private void setFooter()
        {
            dg[0, dg.RowCount - 2].Value = "Envío:";
            dg[0, dg.RowCount - 1].Value = "Obs:";
            subtotalRow = 2;
            for (int i = 0; i < factura.descuentos.Count; i++ )
            {
                dg[3, dg.RowCount - subtotalRow].Value = "Descuento (" + factura.descuentos[i] + "%):";
                subtotalRow++;
            }
            dg[3, dg.RowCount -subtotalRow].Value = "Subtotal:";
            
            dg[3, dg.RowCount - 1].Value = "Total:";
        }

        private void removeLine(int p)
        {
            dg.Rows[p].SetValues(new string[dg.ColumnCount]);
            updateFooter();
        }
        internal void dg_cellValueEventHandler(object sender, DataGridViewCellEventArgs e)
        {
            if (isCellEditable())
            {
                if (dg.CurrentCell.ColumnIndex < 2)
                {
                    try
                    {
                        int val = Int32.Parse(dg.CurrentCell.Value.ToString());
                        if (val <= 0) dg.CurrentCell.Value = "";
                    }
                    catch
                    {
                        dg.CurrentCell.Value = "";
                    }

                }

            }
        }
        internal void dg_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            if (isCellEditable())
            {
                if (e.Control is TextBox)
                {
                    ((TextBox)e.Control).AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                    ((TextBox)e.Control).AutoCompleteSource = AutoCompleteSource.CustomSource;
                }
                else
                {
                    ((ComboBox)e.Control).AutoCompleteMode = AutoCompleteMode.Suggest;
                    return;
                }
                switch (dg.CurrentCell.ColumnIndex)
                {
                    case 2:
   
                        if (dg.CurrentRow.Index < 32) 
                            ((TextBox)e.Control).AutoCompleteCustomSource = productNameList;
                        break;
                    case 0:
                        ((TextBox)e.Control).AutoCompleteCustomSource = productCodeList;
                        break;
                    default:
                        ((TextBox)e.Control).AutoCompleteCustomSource = null;
                        break;
                }
            }
        }

        private bool isCellEditable()
        {
            return isCellEditable(dg.CurrentCell.ColumnIndex, dg.CurrentCell.RowIndex);
        }
        private bool isCellEditable(int c, int r)
        {
            //if (r == 0 && c == 2) return true;//nombre
            if (r == 3 && c == 2) return true; //remito
            if (r == 4 && c == 4) return true; //nº factura
            if (r > HEADER_LENGTH && c < 3 && r < 32) return true; // productos
            if (r >= 32 && c == 2) return true;
            if (r == 3 && c == 4) return true; //forma de pago
            return false;
        }


        private void dg_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (!factura.facturar()) return;
            
            switch (factura.tFactura)
            {
                case tipoFactura.PRESUPUESTO:
                    //imprimir 3 presupuestos
                    FacturaImpresa = new Factura(factura);
                    print(2, FacturaImpresa);

                    FacturaImpresa = new Factura(factura);
                    FacturaImpresa.tFactura = tipoFactura.NOTA_DE_PEDIDO;
                    print(1, FacturaImpresa);
                    break;

                case tipoFactura.FACTURA_B:
                case tipoFactura.FACTURA_A:
                    //imprimir 1 nota de pedido (100% prod), 2 Factura b (30%, redondeado hacia arriba), 2 presupuesto (70% redondeado hacia abajo)
                    FacturaImpresa = new Factura(factura);
                    FacturaImpresa.tFactura = tipoFactura.NOTA_DE_PEDIDO;
                    print(1, FacturaImpresa);

                    FacturaImpresa = new Factura(factura, true, factura.tFactura, factura.porcentajeFactura);
                    //print(1, FacturaImpresa);
                    print(2, FacturaImpresa);
                    FacturaImpresa = new Factura(factura, false, tipoFactura.PRESUPUESTO, factura.porcentajePPTO);
                    print(2, FacturaImpresa);

                    break;
            }
            
        }

        private void print(int p, Factura facturaImpresa)
        {
            for (int i = 0; i < p; i++)
            {
                if (!Props.imprimir)
                {
                    PrintPreviewDialog objPPdialog = new PrintPreviewDialog();
                    objPPdialog.Document = printDocument1;
                    objPPdialog.ShowDialog();
                }
                else
                {
                    PrintDialog printDialog = new PrintDialog();
                    printDialog.Document = printDocument1;
                    printDialog.UseEXDialog = true;

                    //Get the document
                    if (DialogResult.OK == printDialog.ShowDialog())
                    {
                        printDocument1.DocumentName = "Test Page Print"; //?
                        printDocument1.Print();
                    }
                }
            }
        }

        private void printDocument1_BeginPrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            try
            {
                strFormat = new StringFormat();
                strFormat.LineAlignment = StringAlignment.Center;
                strFormat.Trimming = StringTrimming.EllipsisCharacter;
                arrColumnLefts.Clear();
                arrColumnWidths.Clear();
                iCellHeight = 0;
                iRow = 0;
                bFirstPage = true;
                

                // Calculating Total Widths
                iTotalWidth = 0;
                foreach (DataGridViewColumn dgvGridCol in dg.Columns)
                {
                    iTotalWidth += dgvGridCol.Width;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        #region printpage
        private void printDocument1_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {

            /*try
            {*/
            //Set the left margin
            int iLeftMargin = 25;
            //Set the top margin
            int iTopMargin = e.MarginBounds.Top;
            e.HasMorePages = false;
            int iTmpWidth = 0;

            //For the first page to print set the cell width and header height
            if (bFirstPage)
            {
                foreach (DataGridViewColumn GridCol in dg.Columns)
                {
                    iTmpWidth = (int)(Math.Floor((float)((float)GridCol.Width / (float)iTotalWidth * (float)iTotalWidth * ((float)e.MarginBounds.Width / (float)iTotalWidth))));

                    iHeaderHeight = 0; //alto de la fila header... 
                    //(int)(e.Graphics.MeasureString(GridCol.HeaderText,GridCol.InheritedStyle.Font, iTmpWidth).Height) + 11;

                    arrColumnLefts.Add(iLeftMargin);
                    arrColumnWidths.Add(iTmpWidth);
                    iLeftMargin += iTmpWidth;
                }
            }
            while (iRow <= rowCount)
            {
                formatString(iRow);
                DataGridViewRow GridRow = dg.Rows[iRow];
                //Set the cell height
                iCellHeight = GridRow.Height + 5;
                int iCount = 0;
                if (iRow == 0)
                {
                    if (FacturaImpresa.tFactura == tipoFactura.NOTA_DE_PEDIDO)
                    e.Graphics.DrawString("Nota de pedido", new Font(dg.Font, FontStyle.Bold), Brushes.Black, e.MarginBounds.Left,
                    e.MarginBounds.Top - e.Graphics.MeasureString("Nota de pedido", new Font(dg.Font, FontStyle.Bold), e.MarginBounds.Width).Height - 20);
                    //Draw Columns                 
                    iTopMargin = 80; // e.MarginBounds.Top; //header top margin
                    foreach (DataGridViewColumn GridCol in dg.Columns)
                    {
                        e.Graphics.DrawString(GridCol.HeaderText, GridCol.InheritedStyle.Font, new SolidBrush(GridCol.InheritedStyle.ForeColor), new RectangleF((int)arrColumnLefts[iCount], iTopMargin,
                            (int)arrColumnWidths[iCount], iHeaderHeight), strFormat);
                        iCount++;
                    }

                    iTopMargin += iHeaderHeight;
                }
                iCount = 0;
                //Draw Columns Contents                
                foreach (DataGridViewCell Cel in GridRow.Cells)
                {
                    if (cellDrawable(Cel.ColumnIndex, Cel.RowIndex))
                        e.Graphics.DrawString(getCellValue(Cel.ColumnIndex, Cel.RowIndex), Cel.InheritedStyle.Font, new SolidBrush(Cel.InheritedStyle.ForeColor), new RectangleF((int)arrColumnLefts[iCount],
                            (float)iTopMargin, (int)arrColumnWidths[iCount], (float)iCellHeight), strFormat);

                    //Drawing Cells Borders 
                    e.Graphics.DrawRectangle(Pens.Black, new Rectangle((int)arrColumnLefts[iCount], iTopMargin, (int)arrColumnWidths[iCount], iCellHeight));
                    iCount++;
                }
                iRow++;
                iTopMargin += iCellHeight;
            }
            /*}
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message, "Error", MessageBoxButtons.OK,
                   MessageBoxIcon.Error);
            }*/
        }

        private string getCellValue(int c, int r)
        {

            if (r <= 6)
                if (dg.Rows[r].Cells[c].Value == null)
                    return "";
                else
                    return dg.Rows[r].Cells[c].Value.ToString();

            if (r <= 25) //productos
            {
                if (r <= FacturaImpresa.productList.Count + 6)
                {
                    switch (c)
                    {
                        case 0: //codigo
                        case 2: //nombre
                            if (dg.Rows[r].Cells[c].Value == null) return "";
                            return dg.Rows[r].Cells[c].Value.ToString(); 
                        case 3: //precio unitario
                            if (FacturaImpresa.tFactura == tipoFactura.FACTURA_A)
                                return FacturaImpresa.productList[r - 7].p.precioNOIVA.ToString();
                            else
                                return FacturaImpresa.productList[r - 7].p.precioIVA.ToString();
                        case 1: //cantidad
                            return FacturaImpresa.productList[r - 7].cantidad.ToString();
                        case 4:
                            if (FacturaImpresa.tFactura == tipoFactura.FACTURA_A)
                                return (FacturaImpresa.productList[r - 7].p.precioNOIVA * FacturaImpresa.productList[r - 7].cantidad).ToString();
                            else
                                return (FacturaImpresa.productList[r - 7].p.precioIVA* FacturaImpresa.productList[r - 7].cantidad).ToString();
                    }
                }
                if (dg.Rows[r].Cells[c].Value == null) return ""; 
                return dg.Rows[r].Cells[c].Value.ToString();
            }

            if (r == (rowCount-2) && c == 2)
                    return new Numalet().ToCustomCardinal(FacturaImpresa.getTotal());

            
                if (c == 4 || c ==3)
                {
                    return FacturaImpresa.cellValueAt(c, r, rowCount);
                }
                else
                {
                    if (dg.Rows[r].Cells[c].Value == null) return "";
                    return dg.Rows[r].Cells[c].Value.ToString(); //celdas con "subtotal", etc
                }
            

            
        }

        private bool cellDrawable(int col, int row)
        {
            if (FacturaImpresa.tFactura == tipoFactura.FACTURA_A || FacturaImpresa.tFactura == tipoFactura.FACTURA_B)
            {
                if (row >= 6) return true;
                if (col == 1 || col == 3) return false;
            }
            return true;
        }

        private void formatString(int iRow)
        {
            if (iRow < 6)
                strFormat.Alignment = StringAlignment.Near;
            else
                strFormat.Alignment = StringAlignment.Center;
        }
#endregion
    }
}
