using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApiGintec.Repository.Tables;

namespace WebApiGintec.Application.Campeonato.Models
{
    public class CampeonatoJogoReponse : CampeonatoJogo
    {
        public string Nome1 { get; set; }
        public string Nome2 { get; set; }
        public string? Vencedor { get; set; }        
    }
}
