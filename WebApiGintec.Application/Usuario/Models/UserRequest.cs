using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApiGintec.Repository.Tables;

namespace WebApiGintec.Application.Usuario.Models
{
    public class UserRequest
    {
        public string Nome { get; set; }
        public string RM { get; set; }
        public string Email { get; set; }
        public int Status { get; set; }
        public string Senha { get; set; }
        public int SalaCodigo { get; set; }
        public bool IsPadrinho { get; set; }
        public int? AtividadeCodigo { get; set; }
        public int? CampeonatoCodigo { get; set; }  
        public int? OficinaCodigo { get; set; }
        public string? fotoPerfil { get; set; }
    }
}
