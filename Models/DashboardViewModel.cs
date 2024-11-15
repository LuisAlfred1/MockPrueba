namespace MockPrueba.Models
{
    public class DashboardViewModel
    {
        public decimal TotalVentas { get; set; }
        public int TotalUnidades { get; set; }

        public List<VentasPorMes> VentasPorMes { get; set; }
    }

}
