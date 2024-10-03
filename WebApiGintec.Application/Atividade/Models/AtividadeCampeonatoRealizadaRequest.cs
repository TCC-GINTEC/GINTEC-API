using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApiGintec.Application.Atividade.Models
{
    public class AtividadeCampeonatoRealizadaRequest
    {
        public string token { get; set; }        
        public int? AtividadePontuacaoExtraCodigo { get; set; }
        public int? OficinaHorarioCodigo { get; set; }
    }
}
