using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApiGintec.Application.Campeonato.Models
{
    public class OficinaFeitasResponse : Repository.Tables.Oficina
    {
        public bool isRealizada { get; set; }
        public int? OficinaHorarioFuncionamentoCodigo { get; set; }
        public DateTime Data { get; set; }
    }
}
