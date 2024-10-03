using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApiGintec.Repository;
using WebApiGintec.Application.Notificacao;

namespace WebApiGintec.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class NotificacaoController : ControllerBase
    {
        private readonly NotificacaoService _notificacaoService;

        public NotificacaoController(GintecContext ctx)
        {
            _notificacaoService = new NotificacaoService(ctx);
        }
        [HttpGet]
        [Authorize]
        public IActionResult ObterNotificacoes()
        {            
            var response = _notificacaoService.ObterNotificacoes();
          return response.mensagem == "success" ? Ok(response.response) : BadRequest();
        }
    }
}
