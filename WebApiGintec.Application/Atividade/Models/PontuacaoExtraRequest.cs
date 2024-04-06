using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApiGintec.Application.Atividade.Models
{
    public class PontuacaoExtraRequest
    {
        public int Pontuacao { get; set; }

        public int AtividadeCodigo { get; set; }
    }
}
