using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApiGintec.Repository.Tables
{
    public class CampeonatoResultado
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Codigo { get; set; }

        [Required]
        public int JogoCodigo { get; set; }

        [ForeignKey("JogoCodigo")]
        public CampeonatoJogo Jogo { get; set; }

        [Required]
        public int SalaCodigo { get; set; }

        [ForeignKey("SalaCodigo")]
        public Sala Sala { get; set; }

        [Required]
        public int Pontos { get; set; }
    }
}
