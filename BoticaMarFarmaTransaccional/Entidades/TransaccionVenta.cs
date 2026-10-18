using System;

namespace BoticaMarFarmaTransaccional.Entidades
{
    // Datos de una venta: cliente, pago, importe, estado y resumen de productos.
    public class TransaccionVenta
    {
        public string CodigoOperacion { get; set; }
        public Cliente ClienteAsociado { get; set; }
        public string CanalPago { get; set; } // Yape, BCP QR, Agente BCP, Efectivo
        public decimal MontoTotal { get; set; }
        public DateTime FechaHora { get; set; }
        public string Estado { get; set; }
        public string ResumenProductos { get; set; }

        public TransaccionVenta(string codigo, Cliente cliente, string canal, decimal total, string resumen, string estado = "EXITOSA")
        {
            CodigoOperacion = codigo;
            ClienteAsociado = cliente;
            CanalPago = canal;
            MontoTotal = total;
            ResumenProductos = resumen;
            FechaHora = DateTime.Now;
            Estado = estado;
        }

        public override string ToString()
        {
            return $"[{CodigoOperacion}] {FechaHora:HH:mm:ss} | {CanalPago} | {ClienteAsociado.NombreCompleto} | S/ {MontoTotal:F2}";
        }
    }
}