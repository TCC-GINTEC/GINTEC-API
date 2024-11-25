using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApiGintec.Application.Atividade.Models
{
    public class AtividadeRequest
    {
        public string Descricao { get; set; }
        public bool IsPontuacaoExtra { get; set; }
        public int? SalaCodigo { get; set; }
        public int CalendarioCodigo { get; set; }
    }
}
