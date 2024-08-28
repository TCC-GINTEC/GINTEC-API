using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using WebApiGintec.Repository;
using WebApiGintec.Repository.Tables;
using WebApiGintec.Application.Auth.Models;
using WebApiGintec.Application.Util;
using System.Security.Cryptography.Xml;

namespace WebApiGintec.Application.Auth
{
    public class AuthService
    {
        private GintecContext _context;
        public AuthService(GintecContext context)
        {
            _context = context;
        }
        public GenericResponse<LoginResponse> Autenticar(LoginRequest request)
        {
            try
            {
                var user = _context.Usuarios.FirstOrDefault(x => request.email == x.RM && request.password == x.Senha) ?? _context.Usuarios.FirstOrDefault(x => request.email == x.Email && request.password == x.Senha);
                if (user != null)
                {                    
                    return new GenericResponse<LoginResponse>()
                    {
                        mensagem = "success",
                        response = new LoginResponse()
                        {
                            Email = user.Email,
                            Status = user.Status,
                            Token = GeraTokenJwt(user),
                            UsuarioCodigo = user.Codigo,
                            AtividadeCodigo = user.AtividadeCodigo,
                            CampeonatoCodigo = user.CampeonatoCodigo
                        }

                    };
                }                    
                return new GenericResponse<LoginResponse>()
                {
                    mensagem = "user not found",
                };
            }
            catch(Exception ex)
            {
                return new GenericResponse<LoginResponse>()
                {
                    mensagem = "failed",
                    error = ex
                };
            }
            
        }
        private string GeraTokenJwt(Repository.Tables.Usuario usuario)
        {
            var issuer = "etec";
            var audience = "etec";
            var key = "c013239a-5e89-4749-b0bb-07fe4d21710d";

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim("usuarioCodigo", usuario.Codigo.ToString()),
                new Claim("usuarioStatus", usuario.Status.ToString())
            };

            var token = new JwtSecurityToken(issuer: issuer, claims: claims, audience: audience, expires: DateTime.Now.AddDays(1), signingCredentials: credentials);
            var tokenHandler = new JwtSecurityTokenHandler();
            var stringToken = tokenHandler.WriteToken(token);
            return stringToken;

        }
    }
}
