using Amazon.S3;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Security.Claims;
using WebApiGintec.Application.Atividade;
using WebApiGintec.Application.Usuario;
using WebApiGintec.Application.Usuario.Models;
using WebApiGintec.Application.Util;
using WebApiGintec.Repository;
using WebApiGintec.Repository.Tables;

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
        [HttpGet]
        [Route("{id}")]
        [Authorize]
        public IActionResult ObterUsuarioPorCodigo([FromRoute] int id)
        {
            var usuarioService = new UsuarioService(_context);
            var response = usuarioService.ObterUsuarioPorCodigo(id);
            if (response.mensagem == "success")
                return Ok(response.response);
            else
                return BadRequest();
        }
        [HttpGet]
        [Route("Sala/{id}")]
        [Authorize]
        public IActionResult ObterTodosOsUsuariosPorSala([FromRoute] int id)
        {
            var usuarioService = new UsuarioService(_context);
            var response = usuarioService.ObterTodosOsUsuariosPorSala(id);
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
        public IActionResult AtualizarUsuario([FromRoute] int id, [FromBody] UserRequest request, IFormFile? imagem)
        {
            var response = new Usuario();
            if (imagem == null || imagem.Length == 0)
            {
                var usuarioService = new UsuarioService(_context);
                response = usuarioService.AtualizarUsuario(id, request);
            }
            else
            {
                using (var stream = new MemoryStream())
                {
                    imagem.CopyTo(stream);
                    new S3Service("fotojogadores").Upload(stream, id.ToString() + Path.GetExtension(imagem.FileName));
                }
                request.fotoPerfil = id.ToString() + Path.GetExtension(imagem.FileName);

                var usuarioService = new UsuarioService(_context);
                response = usuarioService.AtualizarUsuario(id, request);
            }

            if (response == null)
                return BadRequest();
            else
                return Ok(response);
        }
        [HttpPut]
        [Route("Atualizar")]
        [Authorize]
        public IActionResult AtualizarPerfil([FromForm] PerfilRequest request, [FromForm] IFormFile? imagem)
        {
            var identidade = (ClaimsIdentity)HttpContext.User.Identity;
            var usuarioCodigo = identidade.FindFirst("usuarioCodigo").Value;
            var usuarioService = new UsuarioService(_context);
            var response = new Usuario();
            if (imagem == null || imagem.Length == 0)
            {
                response = usuarioService.AtualizarPerfil(Convert.ToInt32(usuarioCodigo), request);
            }
            else
            {
                using (var stream = new MemoryStream())
                {
                    imagem.CopyTo(stream);

                    request.fotoPerfil = usuarioCodigo + Path.GetExtension(imagem.FileName);
                    response = usuarioService.AtualizarPerfil(Convert.ToInt32(usuarioCodigo), request, stream);
                }
            }

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
                return BadRequest(new { error = "Nenhum usuário com este código" });
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
        public IActionResult ObterPontuacaoGeral(string? id)
        {
            var usuarioService = new UsuarioService(_context);
            var response = usuarioService.ObterPontuacaoTodosAlunos(!string.IsNullOrEmpty(id));

            if (response.mensagem == "success")
                return Ok(response.response);
            else
                return BadRequest();
        }
        [HttpPost]
        [Authorize]
        [Route("InformacoesQRCode")]
        public IActionResult ObterInformacoesQRCode([FromBody] QRCodeRequest request)
        {
            var usuarioService = new UsuarioService(_context);
            var response = usuarioService.ObterInformacoesQRCode(request.Token);

            if (response.mensagem == "success")
                return Ok(response.response);
            else if (response.mensagem == "QR Code Invalid")
                return BadRequest(new { mensagem = "QR Code Inválido" });
            else
                return BadRequest();
        }
        [HttpPost]
        [Authorize]
        [Route("Transferir")]
        public IActionResult TransferirAjudante([FromBody] AjudanteRequest request)
        {
            var identidade = (ClaimsIdentity)HttpContext.User.Identity;
            var usuarioCodigo = identidade.FindFirst("usuarioCodigo").Value;
            var usuarioService = new UsuarioService(_context);
            var response = usuarioService.TransferirAjudante(request, Convert.ToInt32(usuarioCodigo));

            if (response.mensagem == "success")
                return NoContent();
            else
                return BadRequest();
        }
    }
}
