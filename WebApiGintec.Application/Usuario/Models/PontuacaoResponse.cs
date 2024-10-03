using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApiGintec.Application.Usuario.Models
{
    public class PontuacaoResponse
    {
        public int PontuacaGeral { get; set; }
        public int AtividadesFeitos { get; set; }
        public int CampeonatosFeitos { get; set; }
        public int PontuacaoDia { get; set; }
    }
}
