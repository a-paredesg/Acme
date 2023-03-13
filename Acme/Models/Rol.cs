using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Acme.Models
{
    public class Rol
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Nombre { get; set; }

        // Relación con la tabla usuario
        public ICollection<Usuario> Usuarios { get; set; }

    }
}