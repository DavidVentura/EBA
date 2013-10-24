using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EBA
{
    class Distribuidor : Cliente
    {
        private string razon_social;
        internal string nombre, localidad, provincia, pais, condPago;
        internal Distribuidor(int id, string razonSocial, string dir, string tel, int desc, string cuit, string iva) : base(id,"","",dir,tel,desc,cuit,iva)
        {
            razon_social = razonSocial;
            tipo = tipoCliente.DISTRIBUIDOR;
        }
        internal Distribuidor(int id, string nombre, string razonSocial, string dir, string tel, int desc, string cuit, string iva)
            : base(id, "", "", dir, tel, desc, cuit, iva)
        {
            this.nombre = nombre;
            razon_social = razonSocial;
            tipo = tipoCliente.DISTRIBUIDOR;
        }
        internal Distribuidor(int id, string nombre, string razonSocial, string dir, string localidad, string tel, string provincia, string pais, string cuit, string condPago, string iva, int desc)
            : base(id, "", "", dir, tel, desc, cuit, iva)
        {
            this.nombre = nombre;
            this.localidad = localidad;
            this.provincia = provincia;
            this.pais = pais;
            this.condPago = condPago;
            razon_social = razonSocial;
            tipo = tipoCliente.DISTRIBUIDOR;
        }

 

        internal override string getNombre()
        {
            return nombre;
        }

        internal override string getRazonSocial()
        {
            return razon_social;
        }
    }
}
