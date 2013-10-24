using System;
using System.Collections.Generic;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
namespace EBA
{
    internal enum tipoFactura
    {
        FACTURA_A,
        FACTURA_B,
        PRESUPUESTO,
        INVALID,
        NOTA_DE_PEDIDO
    }
    internal enum tipoCliente
    {
        CLIENTE,
        DISTRIBUIDOR,
        INVALID,
    }
    internal enum tipoIVA
    {
        RESPONSABLE_INSCRIPTO,
        MONOTRIBUTISTA,
        CONSUMIDOR_FINAL
    }
    internal struct ProductoCantidad
    {
        internal int cantidad;
        internal Producto p;
        internal ProductoCantidad(Producto prod, int c)
        {
            this.cantidad = c;
            this.p = new Producto(prod.codigo,prod.nombre,prod.precioIVA);
        }
    }

    class Factura
    {
        #region variables
        MySqlDataReader r;
        MySqlCommand cmd = Connection.CreateCommand();
        //internal List<Cliente> clientList = new List<Cliente>();
        internal List<Producto> productsList = new List<Producto>();
        internal List<ProductoCantidad> productList = new List<ProductoCantidad>();
        internal List<float> descuentos = new List<float>();
        internal tipoFactura tFactura = tipoFactura.PRESUPUESTO;
        internal Cliente cliente;
        private tipoCliente tCliente = tipoCliente.INVALID;
        internal bool isCC = false;
        internal int porcentajeFactura = 0,porcentajePPTO = 100,numeroFactura = -1;
        private float total = 0, subtotal = 0, ivaAcumulado = 0, aCuenta=0;
        private DateTime fecha = DateTime.Now;
        #endregion

        internal Factura()
        {
            cmd.CommandText = "Select codigo,nombre,precio from productos";
            r = cmd.ExecuteReader();
            while (r.Read())
                productsList.Add(productFromDataReader(r));
            r.Close();
            
            
            r.Close();
        }
        internal Factura(Factura f)
        {
            cliente = f.cliente;
            productList = f.productList;
            tFactura = f.tFactura;
            total = f.total;
            descuentos = f.descuentos;
            
        }

        
        internal Factura(Factura f, bool roundup, tipoFactura t, float percentage = 100)
        {
            cliente = f.cliente;
            tFactura = t;
            ProductoCantidad p;
            total = 0;
            for (int i = 0; i < f.productList.Count; i++)
            {
                p = new ProductoCantidad(f.productList[i].p, f.productList[i].cantidad);
                if (percentage < 100)
                {
                    if (roundup)
                        p.cantidad = (int)Math.Ceiling(f.productList[i].cantidad * (percentage / 100));
                    else
                        p.cantidad = (int)Math.Floor(f.productList[i].cantidad * (percentage / 100));
                }
                productList.Add(p);
                getIvaAcumulado();
                if (tFactura == tipoFactura.FACTURA_A)
                    subtotal += p.cantidad * p.p.precioNOIVA;
                else
                    subtotal += p.cantidad * p.p.precioIVA;
            }
            descuentos = f.descuentos;
            getTotal();
            

        }
        internal bool tryAddProduct(string name)
        {
            Producto p = getProducto(name);
            if (p != null)
            {
                if (productListed(p)) return false;
                productList.Add(new ProductoCantidad(p,0));
                return true;
            }
            return false;
        }
        internal bool tryAddProduct(int id)
        {
            Producto p = getProducto(id);
            if (p != null)
            {
                if (productListed(id)) return false;
                productList.Add(new ProductoCantidad(p,0));
                return true;
            }
            return false;
        }
        internal void setClient(Cliente c)
        {
            cliente = c;
        }

        internal bool productListed(Producto p)
        {
            for (int i = 0; i < productList.Count; i++)
            {
                if (p.codigo == productList[i].p.codigo) return true;
            }
            return false;
        }
        internal bool productListed(int id)
        {
            for (int i = 0; i < productList.Count; i++)
            {
                if (id == productList[i].p.codigo) return true;
            }
            return false;
        }
        internal bool trySetQuantity(int p, object oQuantity)
        {//fixme lento
            //fixme  solo pasa en campos numericos.. en string anda bien.. hay un try-catch fallando?
            if (oQuantity == null) return false;
            if (oQuantity.ToString().Length == 0) return false;
            int q;
            if (!Int32.TryParse(oQuantity.ToString(), out q)) return false;
            for (int i = 0; i < productList.Count; i++)
            {
                if (p == productList[i].p.codigo)
                {
                    ProductoCantidad pc = productList[i];
                    pc.cantidad = q;
                    productList[i] = pc;
                    //productList[i].cantidad = 2;
                    return true;
                }
            }
            return false;
        }
        internal void tryRemoveProduct(object p)
        {
            if (p == null) return;
            if (p.ToString().Length == 0) return;
            int codigo;
            if (!Int32.TryParse(p.ToString(), out codigo)) return;
            for (int i = 0; i < productList.Count; i++)
            {
                if (codigo == productList[i].p.codigo)
                {
                    productList.RemoveAt(i);
                    return;
                }
            }
        }
        internal bool facturar()
        {
            if (!facturaValida()) return false;
            tCliente = cliente.tipo;
            //modal form que da a elegir tipo de factura
            Tipo_de_factura f = new Tipo_de_factura(cliente.iva);
            f.ShowDialog();

            tFactura = f.value();
            porcentajeFactura = f.porcentajeFactura;
            porcentajePPTO = f.porcentajePPTO;
            if (Queries.facturaExiste(numeroFactura))
            {
                MessageBox.Show("Esta factura tiene numero invalido");
                tFactura = tipoFactura.INVALID;
            }
            if (tFactura == tipoFactura.INVALID) return false;
            if (tFactura != tipoFactura.PRESUPUESTO) 
                discountStock();
            guardarFactura(tFactura);
            getTotal();
            if (isCC)
                insertarCC();
            return true;   
        }

        private bool facturaValida()
        {

            if (cliente == null)
            {
                MessageBox.Show("Cliente invalido", "ATENCIÓN", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
                tFactura = tipoFactura.INVALID;
                return false;
            }
            if (productList.Count == 0)
            {
                MessageBox.Show("No hay productos", "ATENCIÓN", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
                tFactura = tipoFactura.INVALID;
                return false;
            }
            int sumProds = 0;
            for (int i = 0; i < productList.Count; i++)
                sumProds += productList[i].cantidad;
            if (sumProds == 0)
            {
                MessageBox.Show("No hay productos (cantidad)", "ATENCIÓN", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
                tFactura = tipoFactura.INVALID;
                return false;
            }
            if (numeroFactura < 0)
            {
                MessageBox.Show("Numero de factura inválido", "ATENCIÓN", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
                tFactura = tipoFactura.INVALID;
                return false;
            }
            return true;
        }

        private void insertarCC()
        {
            if (tCliente == tipoCliente.CLIENTE)
            {
                cmd.CommandText = "insert into cccliente(id,fecha,importe,`a cuenta`, saldo) values(" + getNumeroFactura() + ", CURDATE(), '" + total + "', '" + aCuenta + "'," + (total - aCuenta) + ")";
            }
            else if (tCliente == tipoCliente.DISTRIBUIDOR)
            {
                cmd.CommandText = "insert into ccdistribuidor(id,fecha,importe,`a cuenta`, saldo) values(" + getNumeroFactura() + ", CURDATE(), '" + total + "', '" + aCuenta + "'," + (total - aCuenta) + ")";
                
            }
            cmd.ExecuteNonQuery();
        }
        internal float getDescuento()
        {
            return (subtotal * ((float)cliente.descuento / 100));
        }
        internal float getSubtotal()
        {
            subtotal = 0;
            for (int i = 0; i < productList.Count; i++)
            {
                if (tFactura == tipoFactura.FACTURA_A)
                    subtotal += productList[i].cantidad * productList[i].p.precioNOIVA;
                else
                    subtotal += productList[i].cantidad * productList[i].p.precioIVA;
            }
            return subtotal;
        }
        internal float getTotal()
        {
            total = getSubtotal();
            getIvaAcumulado();
            total += ivaAcumulado; //iva aplica antes de los descuentos
            for (int i = 0; i < descuentos.Count; i++)
            {
                total -= (total * (descuentos[i] / 100));
                ivaAcumulado -= (ivaAcumulado * (descuentos[i] / 100));
            }

            return total;
        }
        internal float getTotalDiscount(int i)
        {
            float salida = getSubtotal() + getIvaAcumulado();
            float tmpSubtotal = salida;

            for (int n = 0; n <= i; n++)
            {
                if (n == 0)
                    salida = (salida * (descuentos[n] / 100));
                else
                {
                    tmpSubtotal -= salida;
                    salida = (tmpSubtotal * (descuentos[n] / 100));

                }

            }
            return salida;
        }
        internal string cellValueAt(int c, int r, int rowCount)
        {
            //Subtotal , <descuentos>, iva (si aplica), total
            r = rowCount - r;
            if (tFactura == tipoFactura.FACTURA_A)
            {
                if (r > descuentos.Count + 2) return ""; //count = descuentos +total, +2 = subtotal, iva
                if (c == 4)
                {
                    if (r == 0)
                    {//total
                        return getTotal().ToString();
                    }
                    else if (r == 1)
                    {
                        return ivaAcumulado.ToString();
                    }
                    else if (r > 1 && r <= 1 + descuentos.Count)
                    {
                        return getTotalDiscount(descuentos.Count + 1 - r).ToString();
                    }
                    else if (r == descuentos.Count + 2)
                    {
                        return subtotal.ToString();
                    }
                }

                else if (c == 3)
                {
                    if (r == 0) return "Total: ";
                    if (r == 1) return "IVA 21%: ";
                    if (r > 1 && r <= (1 + descuentos.Count)) return "Descuento(" + descuentos[descuentos.Count + 1 - r] + "): ";
                    if (r == descuentos.Count + 2) return "Subtotal: ";

                }
            }
            else
            {
                if (c == 4)
                {
                    if (r == 0)
                    {//total
                        return getTotal().ToString();
                    }
                    else if (r >= 1 && r < descuentos.Count + 1)
                    {
                        return getTotalDiscount(descuentos.Count - r).ToString();
                    }
                    else if (r == descuentos.Count + 1)
                    {
                        return getSubtotal().ToString();
                    }
                }

                else if (c == 3)
                {
                    if (r == 0) return "Total: ";
                    if (r >= 1 && r < 1 + descuentos.Count) return "Descuento(" + descuentos[descuentos.Count - r] + "): ";
                    if (r == descuentos.Count + 1) return "Subtotal: ";
                }
            }
            return "";
            /* if (r == rowCount - (FacturaImpresa.descuentos.Count + 1)) //subtotal
                 return FacturaImpresa.getSubtotal().ToString();
             if (r > rowCount - (FacturaImpresa.descuentos.Count + 1) && r < rowCount) //descuentos
                 return FacturaImpresa.getTotalDiscount((rowCount - r) - 1).ToString(); // -r = fila actual, -1 por el indice basado en 0
             if (r == rowCount) //total
                 return FacturaImpresa.getTotal().ToString();*/
        }

        private int getNumeroFactura()
        {
            return numeroFactura;
        }
        private string productNameFromID(int id)
        {
            for (int i = 0; i < productList.Count; i++)
            {
                if (id == productList[i].p.codigo) return productList[i].p.nombre;
            }
            return "";
        }
        private Producto getProducto(int codigo)
        {
            foreach (Producto p in productsList)
            {
                if (p.codigo == codigo) return p;
            }
            return null;
        }
        private Producto getProducto(String name)
        {
            foreach (Producto p in productsList)
            {
                if (p.nombre.ToUpper().Equals(name.ToUpper())) return p;
            }
            return null;
        }

        private Producto productFromDataReader(MySqlDataReader r)
        {
            return new Producto(Int32.Parse(r[0].ToString()), r[1].ToString(), float.Parse(r[2].ToString()));
        }
        private string getTipo(tipoFactura tipo)
        {
            switch (tipo)
            {
                case tipoFactura.FACTURA_A:
                    return "A";
                case tipoFactura.FACTURA_B:
                    return "B";
                case tipoFactura.PRESUPUESTO:
                    return "P";
            }

            return "?";
        }
        private string getColumnaTipo(tipoFactura t)
        {
            switch (t)
            {
                case tipoFactura.FACTURA_A:
                    return "FacturaA";
                case tipoFactura.FACTURA_B:
                    return "FacturaB";
            }
            return "Presupuesto";
        }
        private void guardarFactura(tipoFactura tipo)
        {
            for (int i = 0; i < productList.Count; i++)
                Queries.guardarDatosFactura(numeroFactura, productList[i].p.codigo ,  getTipo(tFactura) , productList[i].p.precioIVA ,productList[i].cantidad);
            //todo fk a cliente en la tabla 
            string tabla = (tCliente == tipoCliente.CLIENTE ? "facturacliente" : "facturadistribuidor");
            Queries.guardarFactura(tabla, cliente.id, numeroFactura, getTipo(tipo));
        }
        private void discountStock()
        {
            foreach (ProductoCantidad pc in productList)
                Queries.descontarStock(pc.p.codigo, pc.cantidad);
        }
        private float getIvaAcumulado()
        {
            float auxIva = 0;
            ivaAcumulado = 0;
            if (tFactura == tipoFactura.FACTURA_A)
            {
                for (int i = 0; i < productList.Count; i++)
                {
                    auxIva = productList[i].p.precioIVA - productList[i].p.precioNOIVA;
                    ivaAcumulado += (auxIva * productList[i].cantidad);
                }
                return ivaAcumulado;

            }
            return 0;
        }
        
    }
}
