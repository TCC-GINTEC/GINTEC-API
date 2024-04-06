using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WebApiGintec.Application.Atividade;
using WebApiGintec.Application.Atividade.Models;
using WebApiGintec.Repository;

namespace WebApiGintec.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AtividadeController : ControllerBase
    {
        private GintecContext _context;
        public AtividadeController(GintecContext context)
        {
            _context = context;
        }
        #region Atividades
        [HttpGet]
        [Authorize]
        public IActionResult ObterTodasAtividades()
        {
            var atividaadeService = new AtividadeService(_context);
            var response = atividaadeService.ObterTodosAtividades();
            if (response.mensagem == "success")
                return Ok(response.response);
            else
                return BadRequest();
        }
        [HttpPost]
        [Authorize]
        public IActionResult InserirAtividade([FromBody] AtividadeRequest request)
        {
            var atividadeService = new AtividadeService(_context);
            var response = atividadeService.InserirAtividade(request);
            if (response.mensagem == "success")
                return Ok(response.response);
            else
                return BadRequest();
        }
        [HttpPut]
        [Authorize]
        [Route("{id}")]
        public IActionResult AtualizarAtividade([FromRoute] int id, [FromBody] AtividadeRequest request)
        {
            var atividadeService = new AtividadeService(_context);
            var response = atividadeService.AtualizarAtividade(id, request);
            if (response.mensagem == "success")
                return Ok(response.response);
            else
                return BadRequest();
        }
        [HttpDelete]
        [Authorize]
        [Route("{id}")]
        public IActionResult DeletarAtividade([FromRoute] int id)
        {
            var atividadeService = new AtividadeService(_context);
            var response = atividadeService.DeletarAtividade(id);
            if (response.mensagem == "success")
            {
                if (response.response)
                    return NoContent();
                else
                    return BadRequest(new { error = "User not exists" });
            }
            else
                return BadRequest();
        }
        [HttpPost]
        [Authorize]
        [Route("MarcarPontos")]
        public IActionResult MarcarPontuação([FromBody] AtividadeCampeonatoRealizadaRequest request)
        {
            var atividadeService = new AtividadeService(_context);
            var identidade = (ClaimsIdentity)HttpContext.User.Identity;
            var usuarioCodigo = identidade.FindFirst("usuarioCodigo").Value;
            var response = atividadeService.MarcarPontos(request, Convert.ToInt32(usuarioCodigo));
            if (response.mensagem == "success")
                return Ok(response.response);
            else if (response.mensagem == "Score already marked")
                return BadRequest(new {error = "Você já jogou este jogo/campeonato"});
            else
                return BadRequest();
        }
        #endregion
        #region Pontuacao
        [HttpGet]
        [Authorize]
        [Route("Pontuacao/{id}")]
        public IActionResult ObterPontuacaoExtraPorAtividade([FromRoute] int id)
        {
            var atividaadeService = new AtividadeService(_context);
            var response = atividaadeService.ObterPontuacaoExtraPorAtividade(id);
            if (response.mensagem == "success")
                return Ok(response.response);
            else
                return BadRequest();
        }
        [HttpPost]
        [Authorize]
        [Route("Pontuacao")]
        public IActionResult InserirPontuacaoExtra([FromBody] PontuacaoExtraRequest request)
        {
            var atividadeService = new AtividadeService(_context);
            var response = atividadeService.InserirPontuacaoExtra(request);
            if (response.mensagem == "success")
                return Ok(response.response);
            else
                return BadRequest();
        }       
        [HttpDelete]
        [Authorize]
        [Route("Pontuacao/{id}")]
        public IActionResult DeletarPontuacaoExtra([FromRoute] int id)
        {
            var atividadeService = new AtividadeService(_context);
            var response = atividadeService.DeletarPontuacaoExtra(id);
            if (response.mensagem == "success")
            {
                if (response.response)
                    return NoContent();
                else
                    return BadRequest(new { error = "Score not exists" });
            }
            else
                return BadRequest();
        }
        #endregion
    }
}
