using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace WebApiGintec.Repository.Tables
{
    public class Campeonato
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Codigo { get; set; }

        [Required]
        [StringLength(50)]
        public string Descricao { get; set; }

        [Required]
        public int SalaCodigo { get; set; }

        [Required]
        public bool isQuadra {  get; set; }
        [Required]        
        public int CalendarioCodigo { get; set; }        
        [ForeignKey("SalaCodigo")]
        public Sala Sala { get; set; }
        [ForeignKey("CalendarioCodigo")]
        public Calendario Calendario { get; set; }
        public List<CampeonatoFase> Fases { get; set; }                
    }
}