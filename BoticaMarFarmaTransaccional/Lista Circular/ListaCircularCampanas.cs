using BoticaMarFarmaTransaccional.Entidades;
using System;

namespace BoticaMarFarmaTransaccional
{
    // Administra campañas en un anillo para poder recorrerlas continuamente.
    public class ListaCircularCampanas
    {
        private NodoCircularCampana cabeza;
        private NodoCircularCampana cola;
        public NodoCircularCampana NodoActual { get; private set; }
        public int TotalCampanas { get; private set; }

        public ListaCircularCampanas()
        {
            cabeza = null;
            cola = null;
            NodoActual = null;
            TotalCampanas = 0;
        }

        // Inserción al anillo en O(1): El nuevo nodo apunta a cabeza y cola lo enlaza
        public void InsertarAlFinal(CampanaSalud campana)
        {
            NodoCircularCampana nuevo = new NodoCircularCampana(campana);

            if (cabeza == null)
            {
                cabeza = nuevo;
                cola = nuevo;
                nuevo.Siguiente = cabeza; // Cierre autorreferenciado
                NodoActual = nuevo;
            }
            else
            {
                cola.Siguiente = nuevo;
                nuevo.Siguiente = cabeza; // Retorno permanente al inicio
                cola = nuevo;
            }
            TotalCampanas++;
        }

        // Desplazamiento rotativo en tiempo constante O(1)
        public CampanaSalud AvanzarSiguiente()
        {
            if (NodoActual != null)
            {
                NodoActual = NodoActual.Siguiente;
                return NodoActual.Dato;
            }
            return null;
        }

        // Devuelve la campaña del nodo actual, o null si todavía no hay ninguna.
        public CampanaSalud ObtenerActual()
        {
            return NodoActual?.Dato;
        }

        // Genera la tabla de inspección de nodos con punteros explícitos
        public string[] ObtenerTrazasDelAnillo()
        {
            if (cabeza == null) return new string[] { "(No existen promociones en el carrusel circular)" };

            string[] lineas = new string[TotalCampanas];
            NodoCircularCampana temp = cabeza;
            int indice = 1;

            do
            {
                string marca = (temp == NodoActual) ? " 👁️ [EN PANTALLA]" : "";
                string destino = (temp == cola)
                    ? $"{cabeza.Dato.Codigo} (Regresa a Cabeza)"
                    : temp.Siguiente.Dato.Codigo;

                lineas[indice - 1] = $"Posición {indice}: [{temp.Dato.Codigo}] {temp.Dato.Titulo} " +
                                     $"| Creado: {temp.Dato.FechaRegistro:dd/MM HH:mm} " +
                                     $"| Vence: {temp.Dato.FechaVigenciaFin:dd/MM/yyyy} " +
                                     $"➔ Enlace Siguiente: [{destino}]{marca}";

                temp = temp.Siguiente;
                indice++;
            } while (temp != cabeza && indice <= TotalCampanas);

            return lineas;
        }
    }
}