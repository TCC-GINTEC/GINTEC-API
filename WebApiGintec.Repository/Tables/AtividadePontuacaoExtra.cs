using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace WebApiGintec.Repository.Tables
{
    public class AtividadePontuacaoExtra
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Codigo { get; set; }

        [Required]
        public int Pontuacao { get; set; }

        [Required]
        public int AtividadeCodigo { get; set; }

        [ForeignKey("AtividadeCodigo")]
        public Atividade? Atividade { get; set; }
    }
}