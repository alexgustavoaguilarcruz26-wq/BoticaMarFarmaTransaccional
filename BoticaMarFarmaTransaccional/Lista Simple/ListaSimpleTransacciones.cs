using BoticaMarFarmaTransaccional.Entidades;
using System;

namespace BoticaMarFarmaTransaccional.ListaSimple
{
   
    // * Lista Enlazada Simple para el registro cronológico general.
    // Asigna memoria dinámica en el Heap bajo demanda e inserta en O(1).
   
    public class ListaSimpleTransacciones
    {
        private NodoSimpleTransaccion cabeza;
        private NodoSimpleTransaccion cola;
        public int TotalOperaciones { get; private set; }

        public ListaSimpleTransacciones()
        {
            cabeza = null;
            cola = null;
            TotalOperaciones = 0;
        }

        // Inserción al final en tiempo constante O(1) usando el puntero cola
        public void InsertarAlFinal(TransaccionVenta transaccion)
        {
            NodoSimpleTransaccion nuevo = new NodoSimpleTransaccion(transaccion);

            if (cabeza == null)
            {
                cabeza = nuevo;
                cola = nuevo;
            }
            else
            {
                cola.Siguiente = nuevo;
                cola = nuevo; // Se desplaza la cola al nuevo elemento
            }
            TotalOperaciones++;
        }

        // Indica si no se ha registrado ninguna transacción.
        public bool EstaVacia()
        {
            return cabeza == null;
        }

        // Recorrido lineal hacia adelante para volcar los datos a la grilla
        public TransaccionVenta[] ObtenerHistorialCompleto()
        {
            TransaccionVenta[] arreglo = new TransaccionVenta[TotalOperaciones];
            NodoSimpleTransaccion actual = cabeza;
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