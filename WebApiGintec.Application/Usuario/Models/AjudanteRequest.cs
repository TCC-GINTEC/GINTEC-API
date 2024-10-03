using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApiGintec.Application.Usuario.Models
{
    public class AjudanteRequest
    {
        public string RM { get; set; }
        public int? atividadeCodigo { get; set; }
        public int? campeonatoCodigo { get; set; }
        public int? oficinaCodigo { get; set; }
    }
}
