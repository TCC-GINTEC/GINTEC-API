using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApiGintec.Application.Usuario.Models
{
    public class PerfilRequest
    {                
        public string Email { get; set; }        
        public string Senha { get; set; }                                
        public string? fotoPerfil { get; set; }
    }
}
