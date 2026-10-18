using BoticaMarFarmaTransaccional.Entidades;

namespace BoticaMarFarmaTransaccional
{
    // Nodo de campaña enlazado con el siguiente elemento del anillo.
    public class NodoCircularCampana
    {
        public CampanaSalud Dato { get; set; }
        public NodoCircularCampana Siguiente { get; set; }

        public NodoCircularCampana(CampanaSalud dato)
        {
            Dato = dato;
            Siguiente = null;
        }
    }
}