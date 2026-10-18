using BoticaMarFarmaTransaccional.Entidades;
using System;

namespace BoticaMarFarmaTransaccional
{
    // Mantiene transacciones enlazadas para recorrerlas hacia delante y hacia atrás.
    public class ListaDobleTransacciones
    {
        private NodoDobleTransaccion cabeza;
        private NodoDobleTransaccion cola;
        public NodoDobleTransaccion NodoActual { get; private set; }
        public int TotalRegistros { get; private set; }

        public ListaDobleTransacciones()
        {
            cabeza = null;
            cola = null;
            NodoActual = null;
            TotalRegistros = 0;
        }

        // * Inserción al final en tiempo constante O(1) sin redimensionar
        public void InsertarAlFinal(TransaccionVenta transaccion)
        {
            NodoDobleTransaccion nuevo = new NodoDobleTransaccion(transaccion);
            if (cabeza == null)
            {
                cabeza = nuevo;
                cola = nuevo;
                NodoActual = nuevo;
            }
            else
            {
                cola.Siguiente = nuevo;
                nuevo.Anterior = cola;
                cola = nuevo;
                NodoActual = nuevo;
            }
            TotalRegistros++;
        }

        // * Navegación bidireccional inmediata en O(1)
        public TransaccionVenta Retroceder()
        {
            if (NodoActual != null && NodoActual.Anterior != null)
            {
                NodoActual = NodoActual.Anterior;
                return NodoActual.Dato;
            }
            return NodoActual?.Dato;
        }

        // Avanza a la siguiente transacción si existe; en caso contrario conserva la actual.
        public TransaccionVenta Avanzar()
        {
            if (NodoActual != null && NodoActual.Siguiente != null)
            {
                NodoActual = NodoActual.Siguiente;
                return NodoActual.Dato;
            }
            return NodoActual?.Dato;
        }

        // Devuelve la transacción seleccionada o null si no hay nodo actual.
        public TransaccionVenta ObtenerActual()
        {
            return NodoActual?.Dato;
        }

        public TransaccionVenta[] ObtenerHistorial()
        {
            TransaccionVenta[] lista = new TransaccionVenta[TotalRegistros];
            NodoDobleTransaccion actual = cabeza;
            int i = 0;
            while (actual != null)
            {
                lista[i++] = actual.Dato;
                actual = actual.Siguiente;
            }
            return lista;
        }
    }
}