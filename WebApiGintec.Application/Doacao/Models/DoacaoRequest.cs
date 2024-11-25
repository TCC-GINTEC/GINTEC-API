using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApiGintec.Application.Doacao.Models
{
    public class DoacaoRequest
    {
        public string Nome { get; set; }
        public DateTime DataLimite { get; set; }
        public int Pontuacao { get; set; }
    }
}
