using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiGintec.Application.Auth;
using WebApiGintec.Application.Auth.Models;
using WebApiGintec.Repository;

namespace WebApiGintec.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly GintecContext _context;

        public AuthController(GintecContext context)
        {
            _context = context;
        }

        [HttpPost]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            var authService = new AuthService(_context);
            var response = authService.Autenticar(request);
            if (response.mensagem == "success")
                return Ok(response.response);
            else if (response.mensagem == "user not found")
                return Unauthorized();
            else
                return BadRequest();
        }
    }
}
