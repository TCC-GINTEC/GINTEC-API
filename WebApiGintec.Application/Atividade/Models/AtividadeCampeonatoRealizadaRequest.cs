using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApiGintec.Application.Atividade.Models
{
    public class AtividadeCampeonatoRealizadaRequest
    {
        public int? AtividadeCodigo { get; set; }

        public int? CampeonatoCodigo { get; set; }

        public int? AtividadePontuacaoExtraCodigo { get; set; }
    }
}
