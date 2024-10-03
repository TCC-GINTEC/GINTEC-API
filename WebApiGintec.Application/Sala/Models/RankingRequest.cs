using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApiGintec.Application.Sala.Models
{
    public class RankingRequest
    {
        public DateTime FiltroData { get; set; }    
        public bool IsPadrinho { get; set; }
    }
}
