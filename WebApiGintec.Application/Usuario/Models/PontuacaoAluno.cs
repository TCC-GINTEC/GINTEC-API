using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApiGintec.Application.Usuario.Models
{
    public class PontuacaoAluno
    {
        public string FotoPerfil { get; set; }
        public string Nome { get; set; }
        public string Turma { get; set; }
        public bool isPadrinho { get; set; }
        public PontuacaoResponse Pontos { get; set; }
    }
}
