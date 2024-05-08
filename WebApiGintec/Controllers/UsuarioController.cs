using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebApiGintec.Application.Atividade;
using WebApiGintec.Application.Usuario;
using WebApiGintec.Application.Usuario.Models;
using WebApiGintec.Repository;

namespace WebApiGintec.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UsuarioController : ControllerBase
    {
        private readonly GintecContext _context;

        public UsuarioController(GintecContext context)
        {
            _context = context;
        }
        [HttpGet]
        [Authorize]
        public IActionResult ObterTodosOsUsuarios()
        {
            var usuarioService = new UsuarioService(_context);
            var response = usuarioService.ObterTodosOsUsuarios();
            if (response.mensagem == "success")
                return Ok(response.response);
            else
                return BadRequest();
        }
        [HttpPost]
        [Authorize]
        public IActionResult InserirUsuario([FromBody] UserRequest request)
        {
            var usuarioService = new UsuarioService(_context);
            var response = usuarioService.InserirUsuario(request);
            if (response == null)
                return BadRequest();
            else
                return Ok(response);
        }
        [HttpPut]
        [Route("{id}")]
        [Authorize]
        public IActionResult AtualizarUsuario([FromRoute] int id, [FromBody] UserRequest request)
        {
            var usuarioService = new UsuarioService(_context);
            var response = usuarioService.AtualizarUsuario(id, request);
            if (response == null)
                return BadRequest();
            else
                return Ok(response);
        }
        [HttpDelete]
        [Route("{id}")]
        [Authorize]
        public IActionResult DeletarUsuario([FromRoute] int id)
        {
            var usuarioService = new UsuarioService(_context);
            bool response = usuarioService.DeletarUsuario(id);
            if (response)
                return NoContent();
            else
                return BadRequest(new {error= "Nenhum usuário com este código"});
        }
        [HttpGet]
        [Authorize]
        [Route("ObterPontuacao")]
        public IActionResult ObterPontuacao()
        {            
            var identidade = (ClaimsIdentity)HttpContext.User.Identity;
            var usuarioCodigo = identidade.FindFirst("usuarioCodigo").Value;
            var usuarioService = new UsuarioService(_context);
            var response = usuarioService.ObterPontuacao(Convert.ToInt32(usuarioCodigo));

            if (response.mensagem == "success")
                return Ok(response.response);
            else
                return BadRequest();
        }
        [HttpGet]
        [Authorize]
        [Route("ObterPontuacaoGeral")]
        public IActionResult ObterPontuacaoGeral()
        {            
            var usuarioService = new UsuarioService(_context);
            var response = usuarioService.ObterPontuacaoTodosAlunos();

            if (response.mensagem == "success")
                return Ok(response.response);
            else
                return BadRequest();
        }
    }
}
