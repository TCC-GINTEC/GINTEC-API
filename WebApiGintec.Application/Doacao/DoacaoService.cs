using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApiGintec.Application.Doacao.Models;
using WebApiGintec.Application.Util;
using WebApiGintec.Repository;

namespace WebApiGintec.Application.Doacao
{
    public class DoacaoService
    {
        private GintecContext _context;
        public DoacaoService(GintecContext context)
        {
            _context = context;
        }
        public GenericResponse<bool> InserirDoacao(DoacaoRequest request)
        {
            try
            {
                _context.Doacao.Add(new Repository.Tables.Doacao()
                {
                    DateLimite = request.DataLimite,
                    Nome = request.Nome,
                    Pontuacao = request.Pontuacao,
                });
                _context.SaveChanges();
                return new GenericResponse<bool>()
                {
                    mensagem = "success",
                    response = true
                };
            }
            catch(Exception ex)
            {
                return new GenericResponse<bool>()
                {
                    response = false,
                    mensagem = "failed",
                    error = ex
                };
            }
        }
        public GenericResponse<List<Repository.Tables.Doacao>> ObterDoacao()
        {
            try
            {                                
                return new GenericResponse<List<Repository.Tables.Doacao>>()
                {
                    mensagem = "success",
                    response = _context.Doacao.ToList()
                };
            }
            catch(Exception ex)
            {
                return new GenericResponse<List<Repository.Tables.Doacao>>
                {
                    response = null,
                    mensagem = "failed",
                    error = ex
                };
            }
        } 
        public GenericResponse<Repository.Tables.Doacao> ObterDoacaoPorCodigo(int codigo)
        {
            try
            {                                
                return new GenericResponse<Repository.Tables.Doacao>()
                {
                    mensagem = "success",
                    response = _context.Doacao.Include(x => x.DoacaoAluno).ThenInclude(x => x.Usuario).FirstOrDefault(x => x.Codigo == codigo)
                };
            }
            catch(Exception ex)
            {
                return new GenericResponse<Repository.Tables.Doacao>
                {
                    response = null,
                    mensagem = "failed",
                    error = ex
                };
            }
        }
        public GenericResponse<bool> AtualizarDoacao(int codigo, DoacaoRequest request)
        {
            try
            {
                var doacao = _context.Doacao.FirstOrDefault(x => x.Codigo == codigo);

                if (doacao == null)
                {
                    return new GenericResponse<bool>
                    {
                        response = false,
                        mensagem = "Doação não encontrada."
                    };
                }

                doacao.DateLimite = request.DataLimite;
                doacao.Nome = request.Nome;
                doacao.Pontuacao = request.Pontuacao;

                _context.Doacao.Update(doacao);
                _context.SaveChanges();

                return new GenericResponse<bool>
                {
                    mensagem = "Doação atualizada com sucesso.",
                    response = true
                };
            }
            catch (Exception ex)
            {
                return new GenericResponse<bool>
                {
                    response = false,
                    mensagem = "Erro ao atualizar a doação.",
                    error = ex
                };
            }
        }
        public GenericResponse<bool> FazerDoacao(DoacaoJogadorRequest request)
        {
            try
            {
                _context.DoacaoAluno.Add(new Repository.Tables.DoacaoAluno()
                {
                    DataCad = DateTime.Now,
                    DoacaoCodigo = request.DoacaoCodigo,
                    UsuarioCodigo = request.UsuarioCodigo
                });
                _context.SaveChanges();

                return new GenericResponse<bool>()
                {
                    mensagem = "success",
                    response = true
                };
            }
            catch(Exception ex)
            {
                return new GenericResponse<bool>()
                {
                    response = false,
                    mensagem = "failed",
                    error = ex
                };
            }
        }
        public GenericResponse<bool> DeletarDoacaoAluno(int doacaoAlunoCodigo)
        {
            try
            {
                var doacaoAluno = _context.DoacaoAluno.FirstOrDefault(x => x.Codigo == doacaoAlunoCodigo);

                if (doacaoAluno == null)
                {
                    return new GenericResponse<bool>
                    {
                        response = false,
                        mensagem = "Doação de aluno não encontrada."
                    };
                }

                _context.DoacaoAluno.Remove(doacaoAluno);
                _context.SaveChanges();

                return new GenericResponse<bool>
                {
                    mensagem = "success",
                    response = true
                };
            }
            catch (Exception ex)
            {
                return new GenericResponse<bool>
                {
                    response = false,
                    mensagem = "Erro ao deletar a doação de aluno.",
                    error = ex
                };
            }
        }
        public GenericResponse<bool> DeletarDoacao(int codigo)
        {
            try
            {                
                var doacao = _context.Doacao.Include(d => d.DoacaoAluno).FirstOrDefault(x => x.Codigo == codigo);

                if (doacao == null)
                {
                    return new GenericResponse<bool>
                    {
                        response = false,
                        mensagem = "Doação não encontrada."
                    };
                }
                
                _context.DoacaoAluno.RemoveRange(doacao.DoacaoAluno);
                
                _context.Doacao.Remove(doacao);

                _context.SaveChanges();

                return new GenericResponse<bool>
                {
                    mensagem = "success",
                    response = true
                };
            }
            catch (Exception ex)
            {
                return new GenericResponse<bool>
                {
                    response = false,
                    mensagem = "Erro ao deletar a doação.",
                    error = ex
                };
            }
        }
    }
}
