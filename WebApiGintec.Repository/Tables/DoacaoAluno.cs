using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApiGintec.Repository.Tables
{
    public class DoacaoAluno
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Codigo { get; set; }

        [Required]
        public int UsuarioCodigo { get; set; }

        [Required]
        public int DoacaoCodigo { get; set; }
        [Required]
        public DateTime DataCad { get; set; }
        
        [ForeignKey("DoacaoCodigo")]
        public Doacao Doacao { get; set; }
        [ForeignKey("UsuarioCodigo")]
        public Usuario Usuario { get; set; }        
    }
}
