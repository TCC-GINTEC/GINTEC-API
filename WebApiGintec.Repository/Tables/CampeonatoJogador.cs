using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApiGintec.Repository.Tables
{
    public class CampeonatoJogador
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Codigo { get; set; }
        public int TimeCodigo { get; set; }
        [Required]
        [ForeignKey("UsuarioCodigo")]
        public int UsuarioCodigo { get; set; }
        [Required]
        [ForeignKey("SalaCodigo")]
        public int SalaCodigo { get; set; }
        [Required]
        [ForeignKey("CampeonatoCodigo")]
        public int CampeonatoCodigo { get; set; }        
        public Usuario Usuario { get; set; }
        public Sala Sala { get; set; }
        public Campeonato Campeonato { get; set; }
    }
}
