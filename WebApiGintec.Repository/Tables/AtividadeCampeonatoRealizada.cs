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
        public int? OficinaCodigo { get; set; }
        public int? AtividadePontuacaoExtraCodigo { get; set; }
        public int? Oficinahorariocodigo { get; set; }
        [Required]
        public DateTime dataCad { get; set; }
        [ForeignKey("UsuarioCodigo")]
        public Usuario Usuario { get; set; }
        [ForeignKey("Oficinahorariocodigo")]
        public OficinaHorarioFuncionamento Oficinahorario { get; set; }
        [ForeignKey("AtividadePontuacaoExtraCodigo")]
        public AtividadePontuacaoExtra? AtividadePontuacaoExtra { get; set; }
        [ForeignKey("CampeonatoCodigo")]
        public CampeonatoFase? CampeonatoFase { get; set; }
        [ForeignKey("AtividadeCodigo")]
        public Atividade? Atividade { get; set; }
        [ForeignKey("OficinaCodigo")]
        public Oficina? Oficina { get; set; }
    }
}