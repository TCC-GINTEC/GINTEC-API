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
                    isPadrinho = request.IsPadrinho

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
        public GenericResponse<List<PontuacaoAluno>> ObterPontuacaoTodosAlunos(bool isPadrinho)
        {
            try
            {
                var lstRank = new List<PontuacaoAluno>();
                
                var lstPontAlunosAtt = (from acr in _context.AtividadesCampeonatoRealizadas
                                        join ap in _context.AtividadesPontuacaoExtra on acr.AtividadePontuacaoExtraCodigo equals ap.Codigo into ap_join
                                        from ap in ap_join.DefaultIfEmpty()
                                        join u in _context.Usuarios on acr.UsuarioCodigo equals u.Codigo
                                        join a in _context.Atividades on acr.AtividadeCodigo equals a.Codigo
                                        join c in _context.Calendario on a.CalendarioCodigo equals c.Codigo
                                        where c.isAtivo == true && u.isPadrinho == isPadrinho
                                        select new
                                        {
                                            AtividadePontuacaoExtra = ap,
                                            Usuario = u,
                                            Atividade = a,
                                            Calendario = c
                                        }).ToList();
                
                var lstPontAlunosChamp = (from acr in _context.AtividadesCampeonatoRealizadas
                                          join u in _context.Usuarios on acr.UsuarioCodigo equals u.Codigo
                                          join fase in _context.Fases on acr.CampeonatoCodigo equals fase.Codigo
                                          join camp in _context.Campeonatos on fase.CampeonatoCodigo equals camp.Codigo
                                          join c in _context.Calendario on camp.CalendarioCodigo equals c.Codigo
                                          where c.isAtivo == true && u.isPadrinho == isPadrinho
                                          select new
                                          {
                                              Campeonato = camp,
                                              Fase = fase,
                                              Usuario = u,
                                              Calendario = c
                                          }).ToList();
                
                var pontosExtras = _context.AlunoPontuacao.Include(x => x.Usuario).ToList();
                
                foreach (var usuario in _context.Usuarios.Include(x => x.Sala).Where(x => x.isPadrinho == isPadrinho).ToList())
                {                    
                    var alunoPontosAtt = lstPontAlunosAtt.Where(x => x.Usuario.Codigo == usuario.Codigo).ToList();
                    int pontosExtra = alunoPontosAtt.Where(x => x.AtividadePontuacaoExtra != null)
                                                    .Select(y => y.AtividadePontuacaoExtra.Pontuacao)
                                                    .Sum();

                    var pontosextraIndividuais = pontosExtras.Where(x => x.Usuario.Codigo == usuario.Codigo).Sum(u => u.Pontos);
                    int pontuacaoAtividades = alunoPontosAtt.Count * 600;

                    var alunoFases = lstPontAlunosChamp.Where(x => x.Usuario.Codigo == usuario.Codigo).ToList();
                    int pontuacaoFases = alunoFases.Count * 350;
                    
                    if (!string.IsNullOrEmpty(usuario.fotoPerfil))
                    {
                        usuario.fotoPerfil = new S3Service("fotojogadores").GetUrlFile(usuario.fotoPerfil, 6);
                    }

                    // Adicionando o aluno ao ranking
                    lstRank.Add(new PontuacaoAluno()
                    {                        
                        Nome = usuario.Nome,
                        isPadrinho = usuario.isPadrinho,                        
                        Turma = usuario.Sala?.Codigo != null? $"{usuario.Sala?.Serie}º {usuario?.Sala.Descricao}" : "",
                        FotoPerfil = usuario.fotoPerfil,
                        Pontos =  new PontuacaoResponse()
                        {
                            PontuacaGeral = pontuacaoAtividades + pontuacaoFases + pontosExtra + pontosextraIndividuais
                        }                        
                    });
                }

                // Retornando a resposta com o ranking ordenado pela pontuação
                return new GenericResponse<List<PontuacaoAluno>>()
                {
                    response = lstRank.OrderByDescending(x => x.Pontos.PontuacaGeral).ToList().Slice(0, 10),
                    mensagem = "success"
                };
            }
            catch (Exception ex)
            {
                // Captura de exceção para retorno de erro
                return new GenericResponse<List<PontuacaoAluno>>()
                {
                    error = ex,
                    mensagem = "failed",
                    response = null
                };
            }
        }

        public GenericResponse<List<PontuacaoAluno>> sdada(bool ispadrinho)
        {
            try
            {
                var usuarios = _context.Usuarios.Include(x => x.Sala).ToList();
                var jogos = _context.AtividadesCampeonatoRealizadas.ToList();
                var pontos = _context.AtividadesPontuacaoExtra.ToList();

                List<PontuacaoAluno> lstResponse = new();

                foreach (var usuario in usuarios)
                {
                    PontuacaoResponse dataResponse = new PontuacaoResponse();
                    var jogosFeitos = jogos.Where(x => x.UsuarioCodigo == usuario.Codigo).ToList();
                    int pontosExtra = pontos.Where(x => jogosFeitos.Select(i => i.AtividadePontuacaoExtraCodigo).Contains(x.Codigo)).Select(n => n.Pontuacao).Sum(u => u);
                    int pontuacaoJogos = jogosFeitos.Count() * 600;

                    var pontosextras = _context.AlunoPontuacao.Where(x => x.UsuarioCodigo == usuario.Codigo).Sum(u => u.Pontos);

                    dataResponse.PontuacaGeral = pontuacaoJogos + pontosExtra + pontosextras;
                    dataResponse.CampeonatosFeitos = jogosFeitos.Where(x => x.CampeonatoCodigo != null).Count();
                    dataResponse.AtividadesFeitos = jogosFeitos.Where(x => x.AtividadeCodigo != null).Count();

                    var photo = "";
                    lstResponse.Add(new PontuacaoAluno
                    {
                        FotoPerfil = photo,
                        Nome = usuario.Nome,
                        Turma = $"{usuario.Sala.Serie}º{usuario.Sala.Descricao}",
                        isPadrinho = usuario.isPadrinho,
                        Pontos = dataResponse
                    });
                }
                foreach (var response in lstResponse.OrderByDescending(x => x.Pontos.PontuacaGeral).ToList().Slice(0, 10))
                {
                    if (!string.IsNullOrEmpty(response.FotoPerfil))
                        response.FotoPerfil = new S3Service("fotojogadores").GetUrlFile(response.FotoPerfil, 6);
                }

                return new GenericResponse<List<PontuacaoAluno>>()
                {
                    mensagem = "success",
                    response = lstResponse.OrderByDescending(x => x.Pontos.PontuacaGeral).ToList().Slice(0, 10)
                };
            }
            catch (Exception ex)
            {
                return new GenericResponse<List<PontuacaoAluno>>()
                {
                    error = ex,
                    mensagem = "failed",
                    response = null
                };

            }
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
    }
}
