using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace WebApiGintec.Repository.Tables
{
    public class AtividadeCampeonatoRealizada
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Codigo { get; set; }

        [Required]
        public int UsuarioCodigo { get; set; }

        public int? AtividadeCodigo { get; set; }

        public int? CampeonatoCodigo { get; set; }

        public int? AtividadePontuacaoExtraCodigo { get; set; }
    }
}