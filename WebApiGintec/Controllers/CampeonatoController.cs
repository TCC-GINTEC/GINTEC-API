using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using WebApiGintec.Application.Campeonato;
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

        [HttpPost]
        [Authorize]
        public IActionResult CriarCampeonato([FromBody] Campeonato campeonato)
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
    }
}
