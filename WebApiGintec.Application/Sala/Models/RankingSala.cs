using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApiGintec.Application.Sala.Models
{
    public class RankingSala
    {
        public int Codigo { get; set; }
        public string Descricao { get; set; }
        public int Pontuacao { get; set; }
    }
}
