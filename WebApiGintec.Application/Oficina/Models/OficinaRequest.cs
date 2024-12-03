using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApiGintec.Application.Oficina.Models
{
    public class OficinaRequest
    {
        public string Descricao { get; set; }
        public int SalaCodigo { get; set; }
        public int CalendarioCodigo { get; set; }
    }
}
