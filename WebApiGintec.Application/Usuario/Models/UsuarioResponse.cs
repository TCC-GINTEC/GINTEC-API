using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApiGintec.Application.Usuario.Models
{
    public class UsuarioResponse : Repository.Tables.Usuario
    {
        public PontuacaoResponse Pontuacao { get; set; }
    }
}
