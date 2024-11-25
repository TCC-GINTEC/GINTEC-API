using Microsoft.AspNetCore.Mvc;
using WebApiGintec.Application.Campeonato;
using WebApiGintec.Application.Doacao.Models;
using WebApiGintec.Application.Doacao;
using WebApiGintec.Repository;

namespace WebApiGintec.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DoacaoController : ControllerBase
    {
        private readonly DoacaoService _doacaoService;

        public DoacaoController(GintecContext context)
        {
            _doacaoService = new DoacaoService(context);
        }

        [HttpPost]
        [Route("InserirDoacao")]
        public IActionResult InserirDoacao([FromBody] DoacaoRequest request)
        {
            var response = _doacaoService.InserirDoacao(request);

            if (response.mensagem == "success")
                return Ok(response.response);
            else
                return BadRequest(response.error);
        }

        [HttpGet]
        [Route("ObterDoacao")]
        public IActionResult ObterDoacao()
        {
            var response = _doacaoService.ObterDoacao();

            if (response.mensagem == "success")
                return Ok(response.response);
            else
                return BadRequest(response.error);
        }

        [HttpGet]
        [Route("ObterDoacaoPorCodigo/{codigo}")]
        public IActionResult ObterDoacaoPorCodigo(int codigo)
        {
            var response = _doacaoService.ObterDoacaoPorCodigo(codigo);

            if (response.mensagem == "success")
                return Ok(response.response);
            else
                return BadRequest(response.error);
        }

        [HttpPut]
        [Route("AtualizarDoacao/{codigo}")]
        public IActionResult AtualizarDoacao(int codigo, [FromBody] DoacaoRequest request)
        {
            var response = _doacaoService.AtualizarDoacao(codigo, request);

            if (response.mensagem == "Doação atualizada com sucesso.")
                return Ok(response.response);
            else
                return BadRequest(response.error);
        }

        [HttpPost]
        [Route("FazerDoacao")]
        public IActionResult FazerDoacao([FromBody] DoacaoJogadorRequest request)
        {
            var response = _doacaoService.FazerDoacao(request);

            if (response.mensagem == "success")
                return Ok(response.response);
            else
                return BadRequest(response.error);
        }

        [HttpDelete]
        [Route("DeletarDoacaoAluno/{doacaoAlunoCodigo}")]
        public IActionResult DeletarDoacaoAluno(int doacaoAlunoCodigo)
        {
            var response = _doacaoService.DeletarDoacaoAluno(doacaoAlunoCodigo);

            if (response.mensagem == "success")
                return Ok(response.response);
            else
                return BadRequest(response.error);
        }

        [HttpDelete]
        [Route("DeletarDoacao/{codigo}")]
        public IActionResult DeletarDoacao(int codigo)
        {
            var response = _doacaoService.DeletarDoacao(codigo);

            if (response.mensagem == "success")
                return Ok(response.response);
            else
                return BadRequest(response.error);
        }
    }
}
