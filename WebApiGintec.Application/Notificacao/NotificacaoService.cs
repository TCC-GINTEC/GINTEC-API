using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApiGintec.Application.Util;
using WebApiGintec.Repository;

namespace WebApiGintec.Application.Notificacao
{
    public class NotificacaoService
    {
        private GintecContext _context;
        public NotificacaoService(GintecContext context)
        {
            _context = context;
        }
        public GenericResponse<List<Repository.Tables.Notificacao>> ObterNotificacoes()
        {
            try
            {
                return new GenericResponse<List<Repository.Tables.Notificacao>>()
                {
                    mensagem = "success",
                    response = _context.Notificacao.OrderBy(x => x.DataCad).ToList()
                };
            }
            catch(Exception ex)
            {
                return new GenericResponse<List<Repository.Tables.Notificacao>>()
                {
                    mensagem = "failed",
                    response = null,
                    error = ex
                };
            }
        }
    }
}
