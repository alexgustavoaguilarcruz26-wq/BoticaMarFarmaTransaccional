using System;

namespace BoticaMarFarmaTransaccional.Entidades
{
    // Producto y cantidad de una venta; su subtotal se calcula con el precio unitario.
    public class DetalleVenta
    {
        public Producto ProductoItem { get; set; }
        public int Cantidad { get; set; }
        public decimal Subtotal=> ProductoItem.PrecioUnitario * Cantidad;

        public DetalleVenta(Producto producto, int cantidad)
        {
            ProductoItem = producto;
            Cantidad = cantidad;
        }
    }
}