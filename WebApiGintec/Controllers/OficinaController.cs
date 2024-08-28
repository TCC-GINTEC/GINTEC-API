using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebApiGintec.Application.Oficina;
using WebApiGintec.Repository;

namespace WebApiGintec.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class OficinaController : ControllerBase
    {
        private OficinaService _oficinaService;
        public OficinaController(GintecContext context)
        {
            _oficinaService = new OficinaService(context);
        }
        [HttpGet]
        [Authorize]
        public IActionResult ObterOficinas()
        {
            var identidade = (ClaimsIdentity)HttpContext.User.Identity;
            var usuarioCodigo = identidade.FindFirst("usuarioCodigo").Value;
            var response = _oficinaService.ObterOficinas(Convert.ToInt32(usuarioCodigo));
            return response.mensagem == "success" ? Ok(response.response) : BadRequest();
        }
    }
}
