using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace WebApiGintec.Repository.Tables
{
    public class ReposicaoUsuario
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Codigo { get; set; }
        [Required]
        public int UsuarioCodigo { get; set; }
        [Required]
        public int CalendarioCodigo { get; set; }
        [ForeignKey("UsuarioCodigo")]
        public Usuario Usuario { get; set; }
        [ForeignKey("CalendarioCodigo")]
        public Calendario Calendario { get; set; }
    }
}
