using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApiGintec.Repository.Tables
{
    public class Doacao
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Codigo { get; set; }

        [Required]
        [MaxLength(50)]
        public string Nome { get; set; }

        [Required]
        public DateTime DateLimite { get; set; }

        [Required]
        public int Pontuacao { get; set; }
        public List<DoacaoAluno>? DoacaoAluno { get; set; }

    }

}
