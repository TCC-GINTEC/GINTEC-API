using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApiGintec.Repository.Tables
{
    public class RankingAluno
    {        
        public string Nome { get; set; }
        public string Sala { get; set; }
        public string FotoPerfil { get; set; }
        public int PontuacaoGeral { get; set; }
    }

}
