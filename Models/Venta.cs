using System.ComponentModel.DataAnnotations.Schema;

namespace MockPrueba.Models
{
    public class Venta
    {
        [Column("id_venta")]
        public int Id { get; set; }

        [Column("fecha_venta")]
        public DateTime FechaVenta { get; set; }

        [Column("id_cliente")]  // Asegúrate de que el nombre de la columna sea el mismo
        public int IdCliente { get; set; }

        [Column("id_producto")]  // Lo mismo para id_producto
        public int IdProducto { get; set; }

        [Column("cantidad")]
        public int Cantidad { get; set; }

        [Column("total_venta")]
        public decimal TotalVenta { get; set; }

        public Cliente? Cliente { get; set; }

        public Producto? Producto { get; set; }
    }
}
