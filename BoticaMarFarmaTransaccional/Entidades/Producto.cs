using System;

namespace BoticaMarFarmaTransaccional.Entidades
{
    // Producto del catálogo con código, nombre y precio unitario.
    public class Producto
    {
        public string Codigo { get; set; }
        public string Nombre { get; set; }
        public decimal PrecioUnitario { get; set; }

        public Producto(string codigo, string nombre, decimal precioUnitario)
        {
            Codigo = codigo;
            Nombre = nombre;
            PrecioUnitario = precioUnitario;
        }

        public override string ToString()
        {
            return $"{Nombre} - S/ {PrecioUnitario:F2}";
        }
    }
}