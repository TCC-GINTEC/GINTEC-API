using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace WebApiGintec.Repository.Tables
{
    public class Usuario
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Codigo { get; set; }

        [Required]
        [StringLength(150)]
        public string Nome { get; set; }

        [Required]
        [StringLength(30)]
        public string RM { get; set; }

        [Required]
        [StringLength(150)]
        public string Email { get; set; }

        [Required]
        public int Status { get; set; }

        [Required]
        [StringLength(100)]
        public string Senha { get; set; }

        [Required]
        public int SalaCodigo { get; set; }
        [Required]
        public bool isPadrinho { get; set; }

        [ForeignKey("SalaCodigo")]
        public Sala Sala { get; set; }

        [ForeignKey("Status")]
        public StatusUsuario StatusUsuario { get; set; }
    }
}