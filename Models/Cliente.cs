using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MockPrueba.Models
{
    public class Cliente
    {
        [Column("id_cliente")]
        public int Id { get; set; }
        [Column("nombre")]
        public string Nombre { get; set; }
        [Column("apellido")]
        public string Apellido { get; set; }

        [Column("correo_electronico")]
        [EmailAddress(ErrorMessage = "Correo electronico Inválido")]
        public string CorreoElectronico { get; set; }
    }
}
