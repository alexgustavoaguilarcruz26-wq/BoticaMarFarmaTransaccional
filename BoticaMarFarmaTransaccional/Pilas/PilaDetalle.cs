using BoticaMarFarmaTransaccional.Entidades;
using System;

namespace BoticaMarFarmaTransaccional.Pilas
{
    // Pila LIFO para almacenar detalles de venta y retirarlos desde la cima.
    public class PilaDetalle
    {
        private NodoDetalle cima;
        public int Cantidad { get; private set; }

        public PilaDetalle()
        {
            cima = null;
            Cantidad = 0;
        }

        // Operación Push: Inserta en la cima en tiempo constante O(1)
        public void Push(DetalleVenta detalle)
        {
            NodoDetalle nuevo = new NodoDetalle(detalle);
            nuevo.Siguiente = cima;
            cima = nuevo;
            Cantidad++;
        }

        // Operación Pop: Extrae de la cima en tiempo constante O(1)
        public DetalleVenta Pop()
        {
            if (EstaVacia())
                return null;

            DetalleVenta datoExtraido = cima.Dato;
            cima = cima.Siguiente;
            Cantidad--;
            return datoExtraido;
        }

        // Consulta el detalle superior sin retirarlo de la pila.
        public DetalleVenta Top()
        {
            return EstaVacia() ? null : cima.Dato;
        }

        // Indica si la pila no contiene detalles.
        public bool EstaVacia()
        {
            return cima == null;
        }

        // Vacía la pila.
        public void Limpiar()
        {
            cima = null;
            Cantidad = 0;
        }

        // Devuelve los detalles en el orden de la cima hacia abajo.
        public DetalleVenta[] ObtenerElementos()
        {
            DetalleVenta[] arreglo = new DetalleVenta[Cantidad];
            NodoDetalle actual = cima;
            int indice = 0;
            while (actual != null)
            {
                arreglo[indice++] = actual.Dato;
                actual = actual.Siguiente;
            }
            return arreglo;
        }
    }
}