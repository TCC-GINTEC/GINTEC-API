using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace WebApiGintec.Repository.Tables
{
    public class Atividade
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Codigo { get; set; }

        [Required]
        [StringLength(50)]
        public string Descricao { get; set; }

        [Required]
        public bool IsPontuacaoExtra { get; set; }

        public int? SalaCodigo { get; set; }
        [Required]
        public int CalendarioCodigo { get; set; }
        [ForeignKey("CalendarioCodigo")]
        public Calendario Calendario { get; set; }
        [ForeignKey("SalaCodigo")]
        public Sala? Sala { get; set; }
        public List<AtividadePontuacaoExtra>? AtividadePontuacaoExtra { get; set; }
    }
}