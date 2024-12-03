using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApiGintec.Repository.Tables
{
    public class Oficina
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Codigo { get; set; }
        [Required]
        public string Descricao { get; set; }
        [Required]
        public int SalaCodigo { get; set; }
        [Required]
        public int CalendarioCodigo { get; set; }
        [Required]
        public bool isAtivo { get; set; }
        [ForeignKey("SalaCodigo")]
        public Sala Sala { get; set; }
        [ForeignKey("CalendarioCodigo")]
        public Calendario Calendario { get; set; }
        public List<OficinaHorarioFuncionamento>? HorariosFuncionamento { get; set; }
        public List<AtividadeCampeonatoRealizada>? AtividadeCampeonatoRealizada { get; set; }
    }

}
