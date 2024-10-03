using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApiGintec.Application.Atividade.Models
{
    public class AtividadesFeitasResponse : Repository.Tables.Atividade
    {
        public bool isRealizada { get; set; }
        public int? pontuacaoExtra { get; set; }
    }
}
