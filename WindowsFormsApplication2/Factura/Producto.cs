using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EBA
{
    class Producto
    {
        internal int codigo;
        internal string nombre;
        internal float precioIVA;
        internal float precioNOIVA;
        internal Producto(int cod, string name, float precioIVA)
        {
            codigo = cod;
            nombre = name;
            this.precioIVA = precioIVA;
            precioNOIVA = precioIVA /1.21f;
        }
    }
}
