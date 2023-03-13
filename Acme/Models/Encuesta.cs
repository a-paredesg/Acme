using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Acme.Models
{
    public class Encuesta
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Nombre { get; set; }

        [Required]
        [StringLength(200)]
        public string Descripcion { get; set; }

        [StringLength(200)]
        public string Url { get; set; }

        // Relacion con la tabla campo
        public ICollection<Campo> Campos { get; set; }
    }
}