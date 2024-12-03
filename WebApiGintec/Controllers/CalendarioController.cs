using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebApiGintec.Application.Atividade;
using WebApiGintec.Application.Auth;
using WebApiGintec.Application.Calendario;
using WebApiGintec.Application.Calendario.Models;
using WebApiGintec.Repository;

namespace WebApiGintec.Controllers
{

    [ApiController]
    [Route("[controller]")]
    public class CalendarioController : ControllerBase
    {
        private readonly GintecContext _context;
        public CalendarioController(GintecContext context)
        {
            _context = context;
        }
        [HttpGet]
        [Authorize]
        public IActionResult ObterDatas()
        {
            var identidade = (ClaimsIdentity)HttpContext.User.Identity;
            var usuariostatus = identidade.FindFirst("usuarioStatus").Value;
            var calendarioService = new CalendarioService(_context);

            var response = usuariostatus == "3" || usuariostatus == "4" ? calendarioService.ObterDatasAdmin() : calendarioService.ObterDatas();
            if (response.mensagem == "success")
                return Ok(response.response);
            else
                return BadRequest();
        }
        [HttpPost]
        [Authorize]
        public IActionResult AdicionarCalendario([FromBody] CalendarioRequest request)
        {
            var calendarioService = new CalendarioService(_context);
            var response = calendarioService.AdicionarCalendario(request);
            return response.mensagem == "success" ? Ok(response.response) : BadRequest(response.error?.Message);
        }

        [HttpPut]
        [Authorize]
        [Route("{id}")]
        public IActionResult AtualizarCalendario([FromRoute] int id, [FromBody] CalendarioRequest request)
        {
            var calendarioService = new CalendarioService(_context);
            var response = calendarioService.AtualizarCalendario(id, request);
            return response.mensagem == "success" ? Ok(response.response) : BadRequest(response.error?.Message);
        }

        [HttpDelete]
        [Authorize]
        [Route("{id}")]
        public IActionResult DeletarCalendario([FromRoute] int id)
        {
            var calendarioService = new CalendarioService(_context);
            var response = calendarioService.DeletarCalendario(id);
            return response.mensagem == "success" ? NoContent() : BadRequest(response.error?.Message);
        }
    }
}
