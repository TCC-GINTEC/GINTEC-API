using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApiGintec.Repository.Tables;

namespace WebApiGintec.Application.Campeonato.Models
{
    public class CampeonatoFeitoResponse : CampeonatoFase
    {
        public enum Status
        {
            Feito,
            Pendente,
            Atual
        }

        public Status Situacao { get; set; }
    }
}
