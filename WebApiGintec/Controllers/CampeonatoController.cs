using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Security.Claims;
using WebApiGintec.Application.Atividade;
using WebApiGintec.Application.Campeonato;
using WebApiGintec.Application.Campeonato.Models;
using WebApiGintec.Repository;
using WebApiGintec.Repository.Tables;

namespace WebApiGintec.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CampeonatoController : ControllerBase
    {
        private readonly CampeonatoService _campeonatoService;

        public CampeonatoController(GintecContext ctx)
        {
            _campeonatoService = new CampeonatoService(ctx);
        }

        [HttpGet]
        [Authorize]
        public IActionResult ObterTodosCampeonatos()
        {
            var response = _campeonatoService.ObterCampeonatos();
            return response.mensagem == "success" ? Ok(response.response) : BadRequest();
        }      
        [HttpGet("{codigo}")]
        [Authorize]
        public IActionResult ObterCampeonato(int codigo)
        {
            var response = _campeonatoService.ObterCampeonatoPorCodigo(codigo);
            return response.mensagem == "success" ? Ok(response.response) : BadRequest(response);
        }
        [HttpGet("Jogos/{codigo}")]
        [Authorize]
        public IActionResult ObterJogos(int codigo)
        {
            var response = _campeonatoService.ObterJogosPorCodigoFase(codigo);
            return response.mensagem == "success" ? Ok(response.response) : BadRequest(response);
        }
        [HttpGet("feito/{codigo}")]
        [Authorize]
        public IActionResult ObterCampeonatoFeito(int codigo)
        {            
            var identidade = (ClaimsIdentity)HttpContext.User.Identity;
            var usuarioCodigo = identidade.FindFirst("usuarioCodigo").Value;
            var response = _campeonatoService.ObterCampeonatoFeito(codigo, Convert.ToInt32(usuarioCodigo));
            return response.mensagem == "success" ? Ok(response.response) : BadRequest(response);
        }
        [HttpPost]
        [Authorize]
        public IActionResult CriarCampeonato([FromBody] CampeonatoRequest campeonato)
        {
            var response = _campeonatoService.CriarCampeonato(campeonato);
            return response.mensagem == "success" ? CreatedAtAction(nameof(ObterCampeonato), new { codigo = response.response.Codigo }, response.response) : BadRequest(response);
        }

        [HttpPut("{codigo}")]
        [Authorize]
        public IActionResult AtualizarCampeonato(int codigo, [FromBody] CampeonatoRequest campeonato)
        {
            var response = _campeonatoService.AtualizarCampeonato(codigo, campeonato);
            return response.mensagem == "success" ? Ok(response.response) : BadRequest(response);
        }

        [HttpDelete("{codigo}")]
        [Authorize]
        public IActionResult DeletarCampeonato(int codigo)
        {
            var response = _campeonatoService.DeletarCampeonato(codigo);
            return response.mensagem == "success" ? (response.response ? NoContent() : BadRequest(new { error = "Campeonato não encontrado" })) : BadRequest(response);
        }
        [HttpPost]
        [Route("Iniciar")]
        [Authorize]
        public IActionResult IniciarCampeonato([FromBody] StartCampeonatoModel request)
        {
            var response = _campeonatoService.IniciarCampeonato(request);
            return response.mensagem == "success" ? (response.response ? NoContent() : BadRequest(new { error = "Campeonato não encontrado" })) : BadRequest(response);
        }
        [HttpGet]
        [Route("Jogador/{salacodigo}/{campeonatocodigo}")]
        [Authorize]
        public IActionResult ObterJogadorPorCampeonatoESala([FromRoute] int salacodigo, [FromRoute] int campeonatocodigo)
        {
            var response = _campeonatoService.ObterJogadorPorCampeonatoESala(salacodigo, campeonatocodigo);
            return response.mensagem == "success" ? (response.response.Count <= 0 ? NoContent() : Ok(response.response)) : BadRequest(response);
        }
        [HttpPost]
        [Route("Jogador")]
        [Authorize]
        public IActionResult CadastrarJogador([FromBody] JogadorRequest request)
        {
            var response = _campeonatoService.CadastrarJogador(request);
            return response.mensagem == "success" ? Ok(response.response) : BadRequest(response);
        }
        [HttpPost]
        [Authorize]
        [Route("Vencedor/{timecodigo}/{fasecodigo}")]
        public IActionResult DefinirVencedor([FromRoute] int timecodigo, [FromRoute] int fasecodigo)
        {
            var response = _campeonatoService.DefinirVencedor(timecodigo, fasecodigo);

            if (response.mensagem == "success")
                return NoContent();
            else 
                return BadRequest(response);
        }
    }
}
