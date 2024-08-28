using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApiGintec.Application.Atividade;
using WebApiGintec.Application.Auth;
using WebApiGintec.Application.Calendario;
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
            var calendarioService = new CalendarioService(_context);
            var response = calendarioService.ObterDatas();
            if (response.mensagem == "success")
                return Ok(response.response);
            else
                return BadRequest();
        }
    }
}
