using BoticaMarFarmaTransaccional.Entidades;

namespace BoticaMarFarmaTransaccional.Pilas
{
    // Nodo de detalle de venta enlazado con el elemento inferior de la pila.
    public class NodoDetalle
    {
        public DetalleVenta Dato { get; set; }
        public NodoDetalle Siguiente { get; set; }

        public NodoDetalle(DetalleVenta dato)
        {
            Dato = dato;
            Siguiente = null;
        }
    }
}