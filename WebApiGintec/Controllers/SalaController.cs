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

        public SalaController(GintecContext context)
        {
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
        [HttpPost]
        [Authorize]
        [Route("Ranking")]
        public IActionResult ObterRanking([FromBody] RankingRequest? request)
        {
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
