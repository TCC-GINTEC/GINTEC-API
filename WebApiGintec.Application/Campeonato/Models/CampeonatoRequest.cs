using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApiGintec.Repository.Tables;

namespace WebApiGintec.Application.Campeonato
{
    public class CampeonatoRequest
    {        
        public string Descricao { get; set; }        
        public int SalaCodigo { get; set; }
        public int CalendarioCodigo { get; set; }
        public bool isQuadra { get; set; }
    }
}

