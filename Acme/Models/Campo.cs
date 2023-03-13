using System.ComponentModel.DataAnnotations;

namespace Acme.Models
{
    public class Campo
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Nombre { get; set; }

        [Required]
        [StringLength(100)]
        public string Titulo { get; set; }

        [Required]
        public bool EsRequerido { get; set; }

        [Required]
        public TipoCampo TipoCampo { get; set; }

        // Relacion con la tabla Encuesta
        public int EncuestaId { get; set; }
        public Encuesta encuesta { get; set; }
    }
}