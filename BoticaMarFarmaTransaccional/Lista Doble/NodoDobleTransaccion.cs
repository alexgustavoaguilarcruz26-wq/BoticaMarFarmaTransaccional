using BoticaMarFarmaTransaccional.Entidades;

namespace BoticaMarFarmaTransaccional
{
    // Nodo de transacción enlazado con el elemento anterior y el siguiente.
    public class NodoDobleTransaccion
    {
        public TransaccionVenta Dato { get; set; }
        public NodoDobleTransaccion Siguiente { get; set; }
        public NodoDobleTransaccion Anterior { get; set; }

        public NodoDobleTransaccion(TransaccionVenta dato)
        {
            Dato = dato;
            Siguiente = null;
            Anterior = null;
        }
    }
}