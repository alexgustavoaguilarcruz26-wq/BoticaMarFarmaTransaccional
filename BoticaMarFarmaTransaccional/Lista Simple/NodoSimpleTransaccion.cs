using BoticaMarFarmaTransaccional.Entidades;

namespace BoticaMarFarmaTransaccional.ListaSimple
{
   
    // * Nodo con puntero único hacia adelante para la Lista Simple.
    
    public class NodoSimpleTransaccion
    {
        public TransaccionVenta Dato { get; set; }
        public NodoSimpleTransaccion Siguiente { get; set; }

        public NodoSimpleTransaccion(TransaccionVenta dato)
        {
            Dato = dato;
            Siguiente = null;
        }
    }
}