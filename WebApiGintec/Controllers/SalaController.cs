using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApiGintec.Application.Sala;
using WebApiGintec.Application.Sala.Models;
using WebApiGintec.Application.Util;
using WebApiGintec.Repository;

namespace WebApiGintec.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SalaController : ControllerBase
    {
        private readonly GintecContext _context;
        private readonly IConfiguration _configuration;       

        public SalaController(IConfiguration configuration, GintecContext context)
        {
            _configuration = configuration;
            _context = context;
        }

        [HttpGet]
        [Authorize]
        public IActionResult ObterTodasAsSalas()
        {
            var sala = new SalaService(_context);
            var response = sala.ObterTodasAsSalas();

            if (response.mensagem == "success")
                return Ok(response.response);
            else
                return BadRequest();
        }
        [HttpGet]
        [Route("{id}")]
        [Authorize]
        public IActionResult ObterSalasPorCodigo([FromRoute] int id)
        {
            var sala = new SalaService(_context);
            var response = sala.ObterSalaPorCodigo(id);

            if (response.mensagem == "success")
                return Ok(response.response);
            else
                return BadRequest();
        }
        [HttpPost]
        [Authorize]
        [Route("Ranking")]
        public IActionResult ObterRanking([FromBody] RankingRequest? request)
        {
            var environmentVariable = _configuration["YourEnvironmentVariableName"];
            if (!string.IsNullOrEmpty(environmentVariable))
            {
                // Retornar 401 se a variável tiver um valor específico
                return Forbid();
            }

            var sala = new SalaService(_context);
            GenericResponse<List<RankingSala>> response;
            if (request is null)
                response = sala.ObterRanking();
            else
                response = sala.ObterRanking(request);
            
            if (response.mensagem == "success")
                return Ok(response.response);
            else
                return BadRequest();
        }
    }
}
