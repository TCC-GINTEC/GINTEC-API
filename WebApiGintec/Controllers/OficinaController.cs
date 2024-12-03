using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebApiGintec.Application.Calendario.Models;
using WebApiGintec.Application.Calendario;
using WebApiGintec.Application.Oficina;
using WebApiGintec.Application.Oficina.Models;
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
        [Route("Feitos")]
        public IActionResult ObterOficinasFeitos()
        {
            var identidade = (ClaimsIdentity)HttpContext.User.Identity;
            var usuarioCodigo = identidade.FindFirst("usuarioCodigo").Value;
            var response = _oficinaService.ObterOficinasFeitos(Convert.ToInt32(usuarioCodigo));
            return response.mensagem == "success" ? Ok(response.response) : BadRequest();
        }
        [HttpGet]
        [Authorize]        
        public IActionResult ObterOficinas()
        {            
            var response = _oficinaService.ObterOficinas();
            return response.mensagem == "success" ? Ok(response.response) : BadRequest();
        }
        [HttpPost]
        [Authorize]
        public IActionResult AdicionarOficina([FromBody] OficinaRequest request)
        {
            var response = _oficinaService.AdicionarOficina(request);
            return response.mensagem == "success" ? Ok(response.response) : BadRequest();
        }
        [HttpPut]
        [Authorize]
        [Route("{id}")]
        public IActionResult AtualizarOficina([FromRoute] int id,[FromBody] OficinaRequest request)
        {
            var response = _oficinaService.AtualizarOficina(id, request);
            return response.mensagem == "success" ? Ok(response.response) : BadRequest();
        }
        [HttpDelete]
        [Authorize]
        [Route("{id}")]
        public IActionResult DeleteOficina([FromRoute] int id)
        {
            var response = _oficinaService.DeletarOficina(id);
            return response.mensagem == "success" ? NoContent() : BadRequest();
        }
        [HttpGet]
        [Authorize]
        [Route("{id}")]
        public IActionResult ObterOficinaPorCodigo([FromRoute] int id)
        {            
            var response = _oficinaService.ObterOficinaPorCodigo(id);
            return response.mensagem == "success" ? Ok(response.response) : BadRequest();
        }
        [HttpGet]
        [Authorize]
        [Route("Feito/{id}")]
        public IActionResult ObterOficinaFeitaPorCodigo([FromRoute] int id)
        {
            var identidade = (ClaimsIdentity)HttpContext.User.Identity;
            var usuarioCodigo = identidade.FindFirst("usuarioCodigo").Value;
            var response = _oficinaService.ObterOficinaFeitaPorCodigo(id, Convert.ToInt32(usuarioCodigo));
            return response.mensagem == "success" ? Ok(response.response) : BadRequest();
        }      
    }
}
