using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System;
using System.Windows.Forms;
using System.Data;
using System.Diagnostics;
using System.Reflection;

namespace EBA
{
    static class Queries
    {
        static MySqlCommand cmd = new MySqlCommand();
        static MySqlDataReader r;
        internal static bool validVersion()
        {
            cmd = new MySqlCommand("getVersion", Connection.con);
            cmd.CommandType = CommandType.StoredProcedure;
            r = cmd.ExecuteReader();
            r.Read();
            string version  = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).FileVersion.Replace(".","");
            int minVersion = r.GetInt16(0);
            r.Close();
            if (minVersion != Int32.Parse(version))
                return false;
            return true;
        }

        internal static List<Distribuidor> getDistribuidores(string p)
        {
            List<Distribuidor> ret = new List<Distribuidor>();
            cmd.CommandText = "Select * from distribuidores where nombre like '%" + p + "%'";
            r = cmd.ExecuteReader();
            while (r.Read())
                ret.Add(new Distribuidor(Int32.Parse(r["id"].ToString()), r["nombre"].ToString(), r["razon social"].ToString(), r["direccion"].ToString(), r["localidad"].ToString(), r["tel"].ToString(), r["provincia"].ToString(), r["pais"].ToString(), r["cuit"].ToString(), r["iva"].ToString(), r["condicion de pago"].ToString(), Int32.Parse(r["desc"].ToString())));

            r.Close();
            return ret;
        }
        internal static List<Cliente> getClients()
        {
            List<Cliente> lc = new List<Cliente>();
            cmd = Connection.CreateCommand();
            cmd.CommandText = "Select id, apellido, nombre,direccion,telefono,descuento,cuit,iva from clientes";
            MySqlDataReader r = cmd.ExecuteReader();
            while (r.Read())
                lc.Add(clientFromDataReader(r));

            cmd.Parameters.Clear();
            r.Close();

            cmd = Connection.CreateCommand();
            cmd.CommandText = "Select id, nombre, rs,direccion,telefono,descuento,cuit,iva from distribuidores";
            
            r = cmd.ExecuteReader();
            while (r.Read())
                lc.Add(distribuidorFromDataReader(r));

            r.Close();
            cmd.Dispose();
            return lc;
        }
        internal static List<Cliente> getClients(string p)
        {
            List<Cliente> lc = new List<Cliente>();
            cmd = Connection.CreateCommand();
            cmd.CommandText = "Select id, apellido, nombre,direccion,telefono,descuento,cuit,iva from clientes where nombre like @filter or apellido like @filter";// or apellido like %@filter%";
            cmd.Parameters.AddWithValue("@filter","%"+p+"%");
           
            MySqlDataReader r = cmd.ExecuteReader();
            while (r.Read())
                lc.Add(clientFromDataReader(r));
            
            cmd.Parameters.Clear();
            r.Close();

            cmd = Connection.CreateCommand();
            cmd.CommandText = "Select id, nombre, rs,direccion,telefono,descuento,cuit,iva from distribuidores where rs like @filter OR nombre like @filter";
            cmd.Parameters.AddWithValue("@filter", "%" + p + "%");
           
            r = cmd.ExecuteReader();
            while (r.Read())
                lc.Add(distribuidorFromDataReader(r));
            
            r.Close();
            cmd.Dispose();
            return lc;
        }
        private static Cliente clientFromDataReader(MySqlDataReader r)
        {
            int aux;
            Int32.TryParse(r[5].ToString(), out aux);
            Cliente c = new Cliente(Int32.Parse(r[0].ToString()), r[1].ToString(), r[2].ToString(), r[3].ToString(), r[4].ToString(), aux, r[6].ToString(), r[7].ToString());
            return c;
        }
        private static Distribuidor distribuidorFromDataReader(MySqlDataReader r)
        {
            int desc;
            Int32.TryParse(r[5].ToString(), out desc);
            Distribuidor c = new Distribuidor(Int32.Parse(r[0].ToString()), r[1].ToString(), r[2].ToString(), r[3].ToString(), r[4].ToString(), desc, r[6].ToString(), r[7].ToString());
            return c;
        }
        internal static TreeNode[] getInsumos(int codProducto)
        {
            List<TreeNode> lt = new List<TreeNode>();
            List<String> categorias = new List<string>(); 
            cmd = Connection.CreateCommand();
            cmd.CommandText = "Select distinct(categoria) from insumos";
            MySqlDataReader r = cmd.ExecuteReader();
            
            while (r.Read())
                categorias.Add(r.GetString(0)); //traigo todas las categorias
            

            foreach (string categoria in categorias) //traigo productos por categoria
            {
                TreeNode t = new TreeNode(categoria);
                cmd = Connection.CreateCommand();
                cmd.CommandText = "Select id, descripcion from insumos where categoria=@categoria";
                cmd.Parameters.AddWithValue("@categoria", categoria);
                r.Close();
                r = cmd.ExecuteReader();
                while (r.Read())
                {
                    TreeNode add = new TreeNode(r.GetString(1));
                    add.Tag = r.GetInt16(0); //id insumo
                    t.Nodes.Add(add);
                }
                lt.Add(t);
            }
            cmd = Connection.CreateCommand();
            cmd.CommandText = "Select idinsumo from insumosproductos where idproducto = @id";
            cmd.Parameters.AddWithValue("@id", codProducto);
            r.Close();
            r = cmd.ExecuteReader();
            while (r.Read())
            {
                for (int i = 0; i < lt.Count; i++)
                {
                    foreach (TreeNode tree in lt[i].Nodes) //categorias
                        if (Int32.Parse(tree.Tag.ToString()) == r.GetInt16(0))
                            tree.Checked = true;
                }
            }
            r.Close();
            return lt.ToArray();
        }
        internal static int guardarDatosFactura(int numeroFactura, int codigo, string tipo, float precio, int cantidad)
        {
            MySqlCommand m = new MySqlCommand("guardarDatosFactura",Connection.con);
            m.Parameters.AddWithValue("NFACTURA", numeroFactura);
            m.Parameters.AddWithValue("CODIGO", codigo);
            m.Parameters.AddWithValue("TIPO", tipo);
            m.Parameters.AddWithValue("PRECIO", precio);
            m.Parameters.AddWithValue("CANTIDAD", cantidad);
            m.CommandType = System.Data.CommandType.StoredProcedure;
            try
            {
                m.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                MessageBox.Show("Hubo un error guardando los datos. Probablemente el numero de factura este repetido. El error es \n: " + e.Message);
                return 0;
            }
            return 1;
        }
        internal static void guardarFactura(string tabla, int id, int numeroFactura, string tipo)
        {
            MySqlCommand m = null;
            switch (tabla)
            {
                case "facturadistribuidor":
                    m = new MySqlCommand("guardarFacturaDistribuidor", Connection.con);
                    break;
                case "facturacliente":
                    m = new MySqlCommand("guardarFacturaCliente", Connection.con);
                    break;
            }
            m.Parameters.AddWithValue("ID", id); 
            m.Parameters.AddWithValue("NFACTURA", numeroFactura);
            m.Parameters.AddWithValue("TIPO", tipo);
            m.CommandType = System.Data.CommandType.StoredProcedure;
            m.ExecuteNonQuery();
        }
        internal static void descontarStock(int codigo, int cantidad)
        {
            MySqlCommand m = new MySqlCommand("descontarStock", Connection.con);
            m.Parameters.AddWithValue("PROD", codigo);
            m.Parameters.AddWithValue("CANT", cantidad);
            m.CommandType = System.Data.CommandType.StoredProcedure;
            m.ExecuteNonQuery();
        }
        internal static void setInsumos(int codigo, int[] p)
        {
            MySqlCommand m = new MySqlCommand("deleteInsumos", Connection.con);
            m.Parameters.AddWithValue("PROD", codigo);
            m.CommandType = System.Data.CommandType.StoredProcedure;
            m.ExecuteNonQuery();
            for (int i = 0; i < p.Length; i++) //mysql no soporta arrays...
            {
                m = new MySqlCommand("setInsumos", Connection.con);
                m.Parameters.AddWithValue("PROD", codigo);
                m.Parameters.AddWithValue("INSUMO", p[i]);
                m.CommandType = System.Data.CommandType.StoredProcedure;
                m.ExecuteNonQuery();
            }
            
        }

        private static MySqlDataAdapter buildDA(string tabla, string filter)
        {
            MySqlDataAdapter da = new MySqlDataAdapter();
            da.SelectCommand = new MySqlCommand("getData", Connection.con);
            da.SelectCommand.Parameters.AddWithValue("TABLA", tabla);
            da.SelectCommand.Parameters.AddWithValue("FILTER", filter);
            da.SelectCommand.CommandType = CommandType.StoredProcedure;
            return da;
        }

        internal static DataTable getInsumosDataTable(out MySqlDataAdapter m, string filter)
        {
            DataTable dt = new DataTable();
            m = buildDA("insumos", filter);
            m.Fill(dt);
            m.InsertCommand = setParametersInsumos(new MySqlCommand("INSERT INTO insumos VALUES(@id,@descripcion,@categoria,@stock)", Connection.con));
            m.DeleteCommand = setParametersInsumos(new MySqlCommand("DELETE FROM insumos WHERE id = @id", Connection.con));
            m.UpdateCommand = setParametersInsumos(new MySqlCommand("UPDATE insumos SET descripcion=@descripcion,categoria=@categoria,stock=@stock WHERE id = @id", Connection.con));
            return dt;
        }
        internal static DataTable getDistribuidoresDataTable(out MySqlDataAdapter m, string filter)
        {
            DataTable dt = new DataTable();
            m = buildDA("distribuidores", filter);
            m.Fill(dt);
            m.InsertCommand = setParametersDistribuidores(new MySqlCommand("INSERT INTO distribuidores VALUES(@id,@nombre,@rs,@direccion,@localidad,@telefono,@provincia,@pais,@cuit,@iva,@condpago,@descuento)", Connection.con));
            m.DeleteCommand = setParametersDistribuidores(new MySqlCommand("DELETE FROM distribuidores WHERE id = @id", Connection.con));
            m.UpdateCommand = setParametersDistribuidores(new MySqlCommand("UPDATE distribuidores SET rs=@rs, nombre=@nombre,direccion=@direccion,localidad=@localidad,telefono=@telefono,provincia=@provincia,pais=@pais,cuit=@cuit,condpago=@condpago,descuento=@descuento WHERE id = @id", Connection.con));
            return dt;
        }
        internal static DataTable getProductosDataTable(out MySqlDataAdapter m, string filter)
        {
            DataTable dt = new DataTable();
            m = buildDA("productos", filter);
            m.Fill(dt);
            m.InsertCommand = setParametersProductos(new MySqlCommand("INSERT INTO productos VALUES(@codigo,@nombre,@precio,@stock)", Connection.con));
            m.DeleteCommand = setParametersProductos(new MySqlCommand("DELETE FROM productos WHERE codigo = @codigo", Connection.con));
            m.UpdateCommand = setParametersProductos(new MySqlCommand("UPDATE productos SET nombre=@nombre,precio=@precio,stock=@stock WHERE codigo = @codigo", Connection.con));
            return dt;
        }
        internal static DataTable getProveedoresDataTable(out MySqlDataAdapter m, string filter)
        {
            DataTable dt = new DataTable();
            m = buildDA("proveedores", filter);
            m.Fill(dt);
            m.InsertCommand = setParametersProveedores(new MySqlCommand("INSERT INTO proveedores VALUES(@id,@rs, @mail,@direccion,@localidad,@telefono,@provincia)", Connection.con));
            m.DeleteCommand = setParametersProveedores(new MySqlCommand("DELETE FROM proveedores WHERE id = @id", Connection.con));
            m.UpdateCommand = setParametersProveedores(new MySqlCommand("UPDATE proveedores SET id=@id,rs=@rs, mail=@mail,direccion=@direccion,localidad=@localidad,telefono=@telefono,provincia=@provincia WHERE id = @id", Connection.con));
            return dt;
        }
        internal static DataTable getClientesDataTable(out MySqlDataAdapter m, string filter)
        {
            DataTable dt = new DataTable();
            m = buildDA("clientes", filter);
            m.Fill(dt);
            m.InsertCommand = setParametersClientes(new MySqlCommand("INSERT INTO clientes VALUES(@id,@apellido, @nombre,@direccion,@localidad,@telefono,@provincia,@pais,@cumpleanos,@descuento,@celular,@cuit,@iva)", Connection.con));
            m.DeleteCommand = setParametersClientes(new MySqlCommand("DELETE FROM clientes WHERE id = @id", Connection.con));
            m.UpdateCommand = setParametersClientes(new MySqlCommand("UPDATE clientes SET apellido=@apellido, nombre=@nombre,direccion=@direccion,localidad=@localidad,telefono=@telefono,provincia=@provincia,pais=@pais,cumpleanos=@cumpleanos,descuento=@descuento,celular=@celular,cuit=@cuit,iva=@iva WHERE id = @id", Connection.con));
            return dt;
        }
        internal static DataTable getTable(string table, string value, out MySqlDataAdapter m)
        {
            table = table.ToLower();
            MySqlDataAdapter da = new MySqlDataAdapter();
            da.SelectCommand = new MySqlCommand("getData", Connection.con);
            da.SelectCommand.Parameters.AddWithValue("TABLA", table);
            da.SelectCommand.Parameters.AddWithValue("FILTER", value);
            da.SelectCommand.CommandType = CommandType.StoredProcedure;
            DataTable dt = new DataTable();
            da.Fill(dt);
            m = da;
            return dt;
        }

        private static MySqlCommand setParametersClientes(MySqlCommand m)
        {
            //todo: largos de variables
            m.Parameters.Add("@id", MySqlDbType.Int16, 5, "id");
            m.Parameters.Add("@apellido", MySqlDbType.String, 45, "apellido");
            m.Parameters.Add("@nombre", MySqlDbType.String, 45, "nombre");
            m.Parameters.Add("@direccion", MySqlDbType.String, 45, "direccion");
            m.Parameters.Add("@localidad", MySqlDbType.String, 45, "localidad");
            m.Parameters.Add("@telefono", MySqlDbType.String, 45, "telefono");
            m.Parameters.Add("@provincia", MySqlDbType.String, 45, "provincia");
            m.Parameters.Add("@pais", MySqlDbType.String, 45, "pais");
            m.Parameters.Add("@cumpleanos", MySqlDbType.String, 45, "cumpleanos");
            m.Parameters.Add("@descuento", MySqlDbType.Int16, 2, "descuento");
            m.Parameters.Add("@celular", MySqlDbType.String, 45, "celular");
            m.Parameters.Add("@cuit", MySqlDbType.String, 45, "cuit");
            m.Parameters.Add("@iva", MySqlDbType.String, 45, "iva");
            return m;
        }
        private static MySqlCommand setParametersInsumos(MySqlCommand m)
        {
            //todo: largos de variables
            m.Parameters.Add("@id", MySqlDbType.Int16, 11, "id");
            m.Parameters.Add("@categoria", MySqlDbType.String, 45, "categoria");
            m.Parameters.Add("@descripcion", MySqlDbType.String, 45, "descripcion");
            m.Parameters.Add("@stock", MySqlDbType.Int16, 11, "stock");
            return m;
        }
        private static MySqlCommand setParametersDistribuidores(MySqlCommand m)
        {
            //todo: largos de variables
            m.Parameters.Add("@id", MySqlDbType.Int16, 5, "id");
            m.Parameters.Add("@rs", MySqlDbType.String, 45, "rs");
            m.Parameters.Add("@nombre", MySqlDbType.String, 45, "nombre");
            m.Parameters.Add("@direccion", MySqlDbType.String, 45, "direccion");
            m.Parameters.Add("@localidad", MySqlDbType.String, 45, "localidad");
            m.Parameters.Add("@telefono", MySqlDbType.String, 45, "telefono");
            m.Parameters.Add("@provincia", MySqlDbType.String, 45, "provincia");
            m.Parameters.Add("@pais", MySqlDbType.String, 45, "pais");
            m.Parameters.Add("@condpago", MySqlDbType.String, 45, "condpago");
            m.Parameters.Add("@cuit", MySqlDbType.String, 45, "cuit");
            m.Parameters.Add("@iva", MySqlDbType.String, 45, "iva");
            m.Parameters.Add("@descuento", MySqlDbType.Int16, 2, "descuento");
            return m;
        }
        private static MySqlCommand setParametersProductos(MySqlCommand m)
        {
            //todo: largos de variables
            m.Parameters.Add("@codigo", MySqlDbType.Int16, 11, "codigo");
            m.Parameters.Add("@nombre", MySqlDbType.String, 45, "nombre");
            m.Parameters.Add("@precio", MySqlDbType.Decimal, 11, "precio");
            m.Parameters.Add("@stock", MySqlDbType.Int16, 11, "stock");
            return m;
        }
        private static MySqlCommand setParametersProveedores(MySqlCommand m)
        {
            //todo: largos de variables
            m.Parameters.Add("@id", MySqlDbType.Int16, 5, "id");
            m.Parameters.Add("@rs", MySqlDbType.String, 45, "rs");
            m.Parameters.Add("@mail", MySqlDbType.String, 45, "mail");
            m.Parameters.Add("@direccion", MySqlDbType.String, 45, "direccion");
            m.Parameters.Add("@localidad", MySqlDbType.String, 45, "localidad");
            m.Parameters.Add("@telefono", MySqlDbType.String, 45, "telefono");
            m.Parameters.Add("@provincia", MySqlDbType.String, 45, "provincia");
            return m;
        }



        internal static bool facturaExiste(int numeroFactura)
        {
            MySqlCommand m = new MySqlCommand("facturaExiste", Connection.con);
            m.Parameters.AddWithValue("NRO", numeroFactura);
            m.CommandType = System.Data.CommandType.StoredProcedure;
            MySqlDataReader r = m.ExecuteReader();
            if (r.Read())
            {
                int val = r.GetInt16(0);
                r.Close();
                return (val==0?false:true);
            }
            return false;
        }
    }
}
