using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Org.BouncyCastle.Asn1.Ocsp;
using Org.BouncyCastle.Bcpg.OpenPgp;
using Org.BouncyCastle.Crypto.Modes.Gcm;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using WebApiGintec.Application.Commom;
using WebApiGintec.Application.Usuario.Models;
using WebApiGintec.Application.Util;
using WebApiGintec.Repository;
using WebApiGintec.Repository.Tables;
using ZstdSharp.Unsafe;
using static System.Net.Mime.MediaTypeNames;

namespace WebApiGintec.Application.Usuario
{
    public class UsuarioService
    {
        private readonly GintecContext _context;

        public UsuarioService(GintecContext context)
        {
            _context = context;
        }

        public Repository.Tables.Usuario AtualizarUsuario(int codigoUsuario, UserRequest request)
        {
            try
            {
                var user = _context.Usuarios.Update(new Repository.Tables.Usuario
                {
                    Codigo = codigoUsuario,
                    Email = request.Email,
                    Nome = request.Nome,
                    RM = request.RM,
                    SalaCodigo = request.SalaCodigo,
                    Senha = request.Senha,
                    Status = request.Status,
                    AtividadeCodigo = request.AtividadeCodigo,
                    CampeonatoCodigo = request.CampeonatoCodigo,
                    isPadrinho = request.IsPadrinho,
                    oficinacodigo = request.OficinaCodigo,

                });
                _context.SaveChanges();

                return user.Entity;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public Repository.Tables.Usuario AtualizarPerfil(int codigoUsuario, PerfilRequest request, Stream imagem = null)
        {
            try
            {
                if (imagem != null)
                {
                    new S3Service("fotojogadores").Upload(imagem, request.fotoPerfil);
                }

                var user = _context.Usuarios.FirstOrDefault(x => x.Codigo == codigoUsuario);
                user.Email = request.Email;
                user.Senha = request.Senha;
                if (!string.IsNullOrEmpty(request.fotoPerfil))
                    user.fotoPerfil = request.fotoPerfil;

                _context.Update(user);
                _context.SaveChanges();

                return user;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool DeletarUsuario(int id)
        {
            try
            {
                var user = _context.Usuarios.FirstOrDefault(x => x.Codigo == id);
                if (user == null)
                    return false;
                _context.Usuarios.Remove(user);
                _context.SaveChanges();

                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public Repository.Tables.Usuario InserirUsuario(UserRequest request)
        {
            try
            {
                var user = _context.Usuarios.Add(new Repository.Tables.Usuario
                {
                    Email = request.Email,
                    Nome = request.Nome,
                    RM = request.RM,
                    SalaCodigo = request.SalaCodigo,
                    Senha = request.Senha,
                    Status = request.Status,
                    isPadrinho = request.IsPadrinho == null ? false : request.IsPadrinho
                });
                _context.SaveChanges();

                return user.Entity;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public GenericResponse<Repository.Tables.Usuario> ObterInformacoesQRCode(string token)
        {
            try
            {
                var qrCode = new QRCodeService().DesencriptarQRCode(token);
                var user = _context.Usuarios.FirstOrDefault(x => x.Codigo == qrCode.UsuarioCodigo && x.Email == qrCode.Email);

                if (!string.IsNullOrEmpty(user.fotoPerfil))
                {
                    user.fotoPerfil = new S3Service("fotojogadores").GetUrlFile(user.fotoPerfil, 1);
                }
                if (user != null)
                    return new GenericResponse<Repository.Tables.Usuario>()
                    {
                        mensagem = "success",
                        response = user
                    };
                return new GenericResponse<Repository.Tables.Usuario>()
                {
                    mensagem = "QR Code Invalid"
                };
            }
            catch (Exception ex)
            {
                return new GenericResponse<Repository.Tables.Usuario>()
                {
                    mensagem = "failed",
                    error = ex
                };
            }
        }

        public GenericResponse<PontuacaoResponse> ObterPontuacao(int codigoUsuario)
        {
            try
            {
                PontuacaoResponse dataResponse = new PontuacaoResponse();
                var jogosFeitos = _context.AtividadesCampeonatoRealizadas.Where(x => x.UsuarioCodigo == codigoUsuario).ToList();
                int pontosExtra = _context.AtividadesPontuacaoExtra.Where(x => jogosFeitos.Select(i => i.AtividadePontuacaoExtraCodigo).Contains(x.Codigo)).Select(n => n.Pontuacao).Sum(u => u);
                int pontuacaoJogos = jogosFeitos.Count() * 300;

                dataResponse.PontuacaGeral = pontuacaoJogos + pontosExtra;
                dataResponse.CampeonatosFeitos = jogosFeitos.Where(x => x.CampeonatoCodigo != null).Count();
                dataResponse.AtividadesFeitos = jogosFeitos.Where(x => x.AtividadeCodigo != null).Count();

                return new GenericResponse<PontuacaoResponse>()
                {
                    mensagem = "success",
                    response = dataResponse
                };
            }
            catch (Exception ex)
            {
                return new GenericResponse<PontuacaoResponse>()
                {
                    mensagem = "failed",
                    response = null,
                    error = ex
                };
            }
        }
        public async Task<GenericResponse<List<PontuacaoAluno>>> ObterPontuacaoTodosAlunos(bool isPadrinho)
        {
            var padrinho = isPadrinho ? '1' : '0';
            var query = @$"
        select u.nome, u.fotoperfil, u.ispadrinho, concat(sala.serie, ""° "", sala.descricao) as turma, sum(pontos) as pontos, sum(atividadesFeitos) as atividadesFeitos, sum(campeonatosFeitos) as campeonatosFeitos from (
select atf.usuariocodigo, (count(atf.codigo) * 600  + sum(exat.pontuacao)) as pontos, count(atf.atividadecodigo) as atividadesFeitos, 0 as campeonatosFeitos from tabatividadecampeonatorealizada as atf
inner join tabatividade as at
on at.codigo = atf.atividadecodigo
left join tabatividadepontuacaoextra as exat
on exat.atividadeCodigo = at.codigo
inner join tabcalendario as c
on c.codigo = at.calendariocodigo
where c.isativo = 1
group by atf.usuariocodigo
union
select atf.usuariocodigo, count(fase.codigo) * 350 as pontos, 0 as atividadesFeitos, count(atf.campeonatocodigo) as campeonatosFeitos from tabatividadecampeonatorealizada as atf
left join tabcampeonatofase as fase 
on fase.codigo = atf.campeonatocodigo
inner join tabcampeonato as camp
on camp.codigo = fase.campeonatocodigo
inner join tabcalendario as c
on c.codigo = camp.calendariocodigo
where c.isativo = 1
group by atf.usuariocodigo
union
select atf.usuariocodigo, (count(off.codigo) * 600) as ponto, 0 as atividadesFeitos, 0 as campeonatosFeitoss from tabatividadecampeonatorealizada as atf
inner join taboficinas as off
on off.codigo = atf.oficinacodigo
group by atf.usuariocodigo) as pontuacaogeral
inner join tabusuario as u 
inner join tabsala as sala
on sala.codigo = u.salacodigo
on u.codigo = usuariocodigo
where u.ispadrinho = {padrinho}
group by usuariocodigo
order by sum(pontos) desc limit 10;";

            var connection = _context.Database.GetDbConnection();

            await using var command = connection.CreateCommand();
            command.CommandText = query;
            command.CommandType = System.Data.CommandType.Text;

            if (connection.State != System.Data.ConnectionState.Open)
                await connection.OpenAsync();

            var result = new List<PontuacaoAluno>();
            await using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                var img = new S3Service("fotojogadores").GetUrlFile(reader["fotoperfil"].ToString(), 6);

                var aluno = new PontuacaoAluno
                {
                    Nome = reader["nome"].ToString(),
                    FotoPerfil = img,
                    Turma = reader["turma"].ToString(),
                    isPadrinho = Convert.ToBoolean(reader["ispadrinho"].ToString()),
                    Pontos = new PontuacaoResponse
                    {
                        PontuacaGeral = Convert.ToInt32(reader["pontos"]),
                        AtividadesFeitos = Convert.ToInt32(reader["atividadesFeitos"]),
                        CampeonatosFeitos = Convert.ToInt32(reader["campeonatosFeitos"]),
                        PontuacaoDia = 0
                    }
                };

                result.Add(aluno);
            }
            return new GenericResponse<List<PontuacaoAluno>>()
            {
                mensagem = "success",
                response = result
            };
        }

        public GenericResponse<List<Repository.Tables.Usuario>> ObterTodosOsUsuarios()
        {
            try
            {
                var response = new GenericResponse<List<Repository.Tables.Usuario>>()
                {
                    mensagem = "success",
                    response = _context.Usuarios.Include(x => x.Sala).Include(c => c.StatusUsuario).ToList()
                };
                return response;
            }
            catch (Exception ex)
            {
                return new GenericResponse<List<Repository.Tables.Usuario>>()
                {
                    mensagem = "failed",
                    response = null,
                    error = ex
                };
            }
        }
        public GenericResponse<List<Repository.Tables.Usuario>> ObterTodosOsUsuariosPorSala(int codigo)
        {
            try
            {
                var response = new GenericResponse<List<Repository.Tables.Usuario>>()
                {
                    mensagem = "success",
                    response = _context.Usuarios.Include(x => x.Sala).Include(c => c.StatusUsuario).Where(y => y.SalaCodigo == codigo).ToList()
                };
                return response;
            }
            catch (Exception ex)
            {
                return new GenericResponse<List<Repository.Tables.Usuario>>()
                {
                    mensagem = "failed",
                    response = null,
                    error = ex
                };
            }
        }

        public GenericResponse<UsuarioResponse> ObterUsuarioPorCodigo(int codigo)
        {
            try
            {
                var user = _context.Usuarios.Include(x => x.Sala)
                    .Include(c => c.StatusUsuario)
                    .Include(c => c.AtividadeCampeonatoRealizada)
                    .ThenInclude(u => u.AtividadePontuacaoExtra)
                    .FirstOrDefault(y => y.Codigo == codigo);

                if (user == null)
                    return new GenericResponse<UsuarioResponse>()
                    {
                        mensagem = "failed"
                    };
                var jogos = _context.AtividadesCampeonatoRealizadas.Include(u => u.AtividadePontuacaoExtra).Where(x => x.UsuarioCodigo == codigo).ToList();


                PontuacaoResponse dataResponse = new PontuacaoResponse();
                int pontosExtra = jogos.Sum(u => u.AtividadePontuacaoExtra?.Pontuacao ?? 0);
                int pontuacaoJogos = jogos.Count() * 600;

                dataResponse.PontuacaGeral = pontuacaoJogos + pontosExtra;
                dataResponse.CampeonatosFeitos = jogos.Where(x => x.CampeonatoCodigo != null).Count();
                dataResponse.AtividadesFeitos = jogos.Where(x => x.AtividadeCodigo != null).Count();
                var pontuacaoDia = jogos.Where(x => x.dataCad.Date == DateTime.Now.Date).Count() * 600 + jogos.Where(x => x.dataCad.Date == DateTime.Now.Date).Sum(u => u.AtividadePontuacaoExtra?.Pontuacao ?? 0);
                dataResponse.PontuacaoDia = pontuacaoDia;

                var photo = "";

                if (!string.IsNullOrEmpty(user.fotoPerfil))
                    user.fotoPerfil = new S3Service("fotojogadores").GetUrlFile(user.fotoPerfil, 6);

                return new GenericResponse<UsuarioResponse>()
                {
                    response = new UsuarioResponse()
                    {
                        Codigo = user.Codigo,
                        Nome = user.Nome,
                        RM = user.RM,
                        Email = user.Email,
                        Status = user.Status,
                        Senha = user.Senha,
                        SalaCodigo = user.SalaCodigo,
                        isPadrinho = user.isPadrinho,
                        fotoPerfil = user.fotoPerfil,
                        oficinacodigo = user.oficinacodigo,
                        Sala = user.Sala,
                        Oficina = user.Oficina,
                        StatusUsuario = user.StatusUsuario,
                        AtividadeCodigo = user.AtividadeCodigo,
                        Atividade = user.Atividade,
                        CampeonatoCodigo = user.CampeonatoCodigo,
                        Campeonato = user.Campeonato,
                        AtividadeCampeonatoRealizada = user.AtividadeCampeonatoRealizada,
                        Pontuacao = dataResponse,
                    },
                    mensagem = "success"
                };
            }
            catch (Exception ex)
            {
                return new GenericResponse<UsuarioResponse>()
                {
                    mensagem = "failed",
                    response = null,
                    error = ex
                };
            }
        }
        public GenericResponse<UsuarioResponse> ObterUsuarioPorRM(string rm)
        {
            try
            {
                var user = _context.Usuarios.Include(x => x.Sala)
                    .Include(c => c.StatusUsuario)
                    .Include(c => c.AtividadeCampeonatoRealizada)
                    .ThenInclude(u => u.AtividadePontuacaoExtra)
                    .FirstOrDefault(y => y.RM == rm);

                if (user == null)
                    return new GenericResponse<UsuarioResponse>()
                    {
                        mensagem = "failed"
                    };
                var jogos = _context.AtividadesCampeonatoRealizadas.Include(u => u.AtividadePontuacaoExtra).Where(x => x.UsuarioCodigo == user.Codigo).ToList();


                PontuacaoResponse dataResponse = new PontuacaoResponse();
                int pontosExtra = jogos.Sum(u => u.AtividadePontuacaoExtra?.Pontuacao ?? 0);
                int pontuacaoJogos = jogos.Count() * 600;

                dataResponse.PontuacaGeral = pontuacaoJogos + pontosExtra;
                dataResponse.CampeonatosFeitos = jogos.Where(x => x.CampeonatoCodigo != null).Count();
                dataResponse.AtividadesFeitos = jogos.Where(x => x.AtividadeCodigo != null).Count();
                var pontuacaoDia = jogos.Where(x => x.dataCad.Date == DateTime.Now.Date).Count() * 600 + jogos.Where(x => x.dataCad.Date == DateTime.Now.Date).Sum(u => u.AtividadePontuacaoExtra?.Pontuacao ?? 0);
                dataResponse.PontuacaoDia = pontuacaoDia;

                var photo = "";

                if (!string.IsNullOrEmpty(user.fotoPerfil))
                    user.fotoPerfil = new S3Service("fotojogadores").GetUrlFile(user.fotoPerfil, 6);

                return new GenericResponse<UsuarioResponse>()
                {
                    response = new UsuarioResponse()
                    {
                        Codigo = user.Codigo,
                        Nome = user.Nome,
                        RM = user.RM,
                        Email = user.Email,
                        Status = user.Status,
                        Senha = user.Senha,
                        SalaCodigo = user.SalaCodigo,
                        isPadrinho = user.isPadrinho,
                        fotoPerfil = user.fotoPerfil,
                        oficinacodigo = user.oficinacodigo,
                        Sala = user.Sala,
                        Oficina = user.Oficina,
                        StatusUsuario = user.StatusUsuario,
                        AtividadeCodigo = user.AtividadeCodigo,
                        Atividade = user.Atividade,
                        CampeonatoCodigo = user.CampeonatoCodigo,
                        Campeonato = user.Campeonato,
                        AtividadeCampeonatoRealizada = user.AtividadeCampeonatoRealizada,
                        Pontuacao = dataResponse,
                    },
                    mensagem = "success"
                };
            }
            catch (Exception ex)
            {
                return new GenericResponse<UsuarioResponse>()
                {
                    mensagem = "failed",
                    response = null,
                    error = ex
                };
            }
        }

        public GenericResponse<bool> TransferirAjudante(AjudanteRequest request, int usuarioCodigo)
        {
            try
            {
                var user = _context.Usuarios.FirstOrDefault(x => x.RM == request.RM);
                if (user is null)
                    return new GenericResponse<bool>()
                    {
                        mensagem = "user not found",
                        response = false
                    };
                user.AtividadeCodigo = request.atividadeCodigo;
                user.CampeonatoCodigo = request.campeonatoCodigo;
                user.oficinacodigo = request.oficinaCodigo;

                var user2 = _context.Usuarios.FirstOrDefault(x => x.Codigo == usuarioCodigo);
                user2.AtividadeCodigo = null;
                user2.CampeonatoCodigo = null;
                user2.oficinacodigo = null;


                _context.Update(user);
                _context.Update(user2);
                _context.SaveChanges();

                return new GenericResponse<bool>()
                {
                    mensagem = "success",
                    response = true
                };
            }
            catch (Exception ex)
            {
                return new GenericResponse<bool>()
                {
                    mensagem = "failed",
                    error = ex,
                };
            }
        }

        public GenericResponse<List<PermissaoStatus>> ObterPermissoes(int usuarioStatus)
        {
            try
            {
                var lstpermissions = _context.PermissaoStatus.Where(x => x.statuscodigo == usuarioStatus).ToList();

                return new GenericResponse<List<PermissaoStatus>>()
                {
                    mensagem = "success",
                    response = lstpermissions
                };
            }
            catch (Exception ex)
            {
                return new GenericResponse<List<PermissaoStatus>>()
                {
                    error = ex,
                    response = null
                };
            }
        }
        public GenericResponse<List<StatusUsuario>> ObterRoles()
        {
            try
            {
                var lstpermissions = _context.Statuses.ToList();

                return new GenericResponse<List<StatusUsuario>>()
                {
                    mensagem = "success",
                    response = lstpermissions
                };
            }
            catch (Exception ex)
            {
                return new GenericResponse<List<StatusUsuario>>()
                {
                    error = ex,
                    response = null
                };
            }
        }
    }
}
