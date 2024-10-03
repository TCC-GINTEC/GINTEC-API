using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApiGintec.Repository.Tables
{
    public class AlunoPontuacao
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Codigo { get; set; }
        [Required]
        public int UsuarioCodigo { get; set; }
        [Required]
        public int Pontos { get; set; }
        [Required]
        public string Justificativa { get; set; }
        [ForeignKey("UsuarioCodigo")]
        public Usuario Usuario { get; set; }
    }
}
