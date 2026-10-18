using System;

namespace BoticaMarFarmaTransaccional.Entidades
{
    // Datos de identificación y nombre de un cliente.
    public class Cliente
    {
        public string Dni { get; set; }
        public string NombreCompleto { get; set; }

        public Cliente(string dni, string nombreCompleto)
        {
            Dni = dni;
            NombreCompleto =nombreCompleto;
        }

        public override string ToString()
        {
            return $"{NombreCompleto} (DNI:  {Dni})";
        }
    }
}