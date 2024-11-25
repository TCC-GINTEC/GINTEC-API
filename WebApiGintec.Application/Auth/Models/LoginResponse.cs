using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApiGintec.Repository.Tables;

namespace WebApiGintec.Application.Auth.Models
{
    public class LoginResponse
    {
        public string Token { get; set; }
        public string Email { get; set; }
        public int UsuarioCodigo { get; set; }        
        public int Status { get; set; }
        public int? AtividadeCodigo { get; set; }
        public int? CampeonatoCodigo { get; set; }
        public int? OficinaCodigo { get; set; }
    }
}
