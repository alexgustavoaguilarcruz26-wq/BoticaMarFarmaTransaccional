using System;

namespace BoticaMarFarmaTransaccional.Entidades
{
    // Campaña con su beneficio, canal y periodo de vigencia.
    public class CampanaSalud
    {
        public string Codigo { get; set; }
       
         public string Titulo { get; set; }
        
        public string BeneficioDescuento { get; set; }
      
        public string CanalAsociado { get; set; }
        
        public DateTime FechaRegistro { get; set; }
         
        public DateTime FechaVigenciaFin { get; set; }

        public CampanaSalud(string codigo, string titulo, string beneficio, string canal, DateTime vigenciaFin)
        {
             Codigo = codigo;
             Titulo = titulo;
             BeneficioDescuento = beneficio;
             CanalAsociado = canal;
             FechaRegistro = DateTime.Now;
             FechaVigenciaFin = vigenciaFin;
        }

        public override string ToString()
        {
            return $"[{Codigo}] {Titulo} |    {BeneficioDescuento} | Vence: {FechaVigenciaFin:dd/MM/yyyy}";
        }
    }
}