using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApiGintec.Repository.Tables
{
    public class OficinaHorarioFuncionamento
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Codigo { get; set; }
        [Required]
        public int OficinaCodigo { get; set; }
        [Required]
        public DateTime Horario { get; set; }
        [ForeignKey("OficinaCodigo")]
        public Oficina Oficina { get; set; }
    }

}
