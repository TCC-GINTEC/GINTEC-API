using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApiGintec.Repository.Tables
{
    public class CampeonatoFase
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Codigo { get; set; }

        [Required]
        [StringLength(50)]
        public string Descricao { get; set; }

        [Required]
        public int CampeonatoCodigo { get; set; }

        [ForeignKey("CampeonatoCodigo")]
        public Campeonato Campeonato { get; set; }

        public List<CampeonatoJogo> Jogos { get; set; }
        public List<AtividadeCampeonatoRealizada>? atividadeCampeonatoRealizadas { get; set; }
    }
}
