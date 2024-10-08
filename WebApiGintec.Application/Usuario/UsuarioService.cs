﻿using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Org.BouncyCastle.Crypto.Modes.Gcm;
using System;
using System.Collections.Generic;
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

        public GenericResponse<List<PontuacaoAluno>> ObterPontuacaoTodosAlunos()
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
                    int pontuacaoJogos = jogosFeitos.Count() * 300;

                    dataResponse.PontuacaGeral = pontuacaoJogos + pontosExtra;
                    dataResponse.CampeonatosFeitos = jogosFeitos.Where(x => x.CampeonatoCodigo != null).Count();
                    dataResponse.AtividadesFeitos = jogosFeitos.Where(x => x.AtividadeCodigo != null).Count();

                    lstResponse.Add(new PontuacaoAluno
                    {
                        Nome = usuario.Nome,
                        Turma = $"{usuario.Sala.Serie}º{usuario.Sala.Descricao}",
                        Pontos = dataResponse
                    });
                }

                return new GenericResponse<List<PontuacaoAluno>>()
                {
                    mensagem = "success",
                    response = lstResponse
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

        public GenericResponse<Repository.Tables.Usuario> ObterUsuarioPorCodigo(int codigo)
        {
            try
            {
                var response = new GenericResponse<Repository.Tables.Usuario>()
                {
                    mensagem = "success",
                    response = _context.Usuarios.Include(x => x.Sala)
                    .Include(c => c.StatusUsuario)
                    .Include(c => c.AtividadeCampeonatoRealizada)
                    .ThenInclude(u => u.AtividadePontuacaoExtra)
                    .FirstOrDefault(y => y.Codigo== codigo)
                };
                return response;
            }
            catch (Exception ex)
            {
                return new GenericResponse<Repository.Tables.Usuario>()
                {
                    mensagem = "failed",
                    response = null,
                    error = ex
                };
            }
        }
    }
}
