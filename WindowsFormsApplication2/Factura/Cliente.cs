using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EBA
{
    class Cliente
    {
        internal int id;
        private string apellido;
        private string nombre;
        internal string direccion;
        private string localidad;
        private string provincia;
        private string pais;
        internal string telefono;
        private DateTime cumple;
        internal int descuento;
        private string celular;
        internal string cuit;
        internal tipoIVA iva;
        internal tipoCliente tipo = tipoCliente.INVALID;
        internal Cliente(int id, string apellido, string nombre, string dir, string tel, int desc, string cuit, string iva)
        {
            this.id = id;
            this.apellido = apellido;
            this.nombre = nombre;
            direccion = dir;
            telefono = tel;
            descuento = desc;
            this.cuit = cuit;
            switch (iva.ToUpper()) {
                case "RESPONSABLE INSCRIPTO":
                    this.iva = tipoIVA.RESPONSABLE_INSCRIPTO;
                    break;
                case "MONOTRIBUTISTA":
                    this.iva = tipoIVA.MONOTRIBUTISTA;
                    break;
                case "CONSUMIDOR FINAL":
                case "S/D":
                    this.iva = tipoIVA.CONSUMIDOR_FINAL;
                    break;
            }

            tipo = tipoCliente.CLIENTE;
        }


        internal virtual string getRazonSocial()
        {
            return "";
        }
        internal virtual string getNombre()
        {
            return apellido + ", " + nombre;
        }
    }
}
