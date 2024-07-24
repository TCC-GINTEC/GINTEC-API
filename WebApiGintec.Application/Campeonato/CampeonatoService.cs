using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using WebApiGintec.Application.Util;
using WebApiGintec.Repository;
using WebApiGintec.Repository.Tables;

namespace WebApiGintec.Application.Campeonato
{
    public class CampeonatoService
    {
        private readonly GintecContext _context;

        public CampeonatoService(GintecContext context)
        {
            _context = context;
        }

        public GenericResponse<Repository.Tables.Campeonato> CriarCampeonato(Repository.Tables.Campeonato campeonato)
        {
            try
            {
                var result = _context.Campeonatos.Add(campeonato);
                _context.SaveChanges();
                return new GenericResponse<Repository.Tables.Campeonato>
                {
                    mensagem = "success",
                    response = result.Entity
                };
            }
            catch (Exception ex)
            {
                return new GenericResponse<Repository.Tables.Campeonato>
                {
                    mensagem = "failed",
                    error = ex
                };
            }
        }

        public GenericResponse<Repository.Tables.Campeonato> AtualizarCampeonato(int codigoCampeonato, CampeonatoRequest campeonato)
        {
            try
            {
                var existingCampeonato = _context.Campeonatos.Find(codigoCampeonato);
                if (existingCampeonato == null)
                {
                    return new GenericResponse<Repository.Tables.Campeonato>
                    {
                        mensagem = "not found"
                    };
                }

                existingCampeonato.Descricao = campeonato.Descricao;
                existingCampeonato.SalaCodigo = campeonato.SalaCodigo;

                _context.Campeonatos.Update(existingCampeonato);
                _context.SaveChanges();

                return new GenericResponse<Repository.Tables.Campeonato>
                {
                    mensagem = "success",
                    response = existingCampeonato
                };
            }
            catch (Exception ex)
            {
                return new GenericResponse<Repository.Tables.Campeonato>
                {
                    mensagem = "failed",
                    error = ex
                };
            }
        }

        public GenericResponse<bool> DeletarCampeonato(int codigoCampeonato)
        {
            try
            {
                var campeonato = _context.Campeonatos.Find(codigoCampeonato);
                if (campeonato == null)
                {
                    return new GenericResponse<bool>
                    {
                        mensagem = "not found",
                        response = false
                    };
                }

                _context.Campeonatos.Remove(campeonato);
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
                    mensagem = "failed",
                    error = ex,
                    response = false
                };
            }
        }

        public GenericResponse<Repository.Tables.Campeonato> ObterCampeonatoPorCodigo(int codigoCampeonato)
        {
            try
            {
                var campeonato = _context.Campeonatos
                    .Include(c => c.Fases)
                    .Include(c => c.Jogos)
                    .Include(c => c.Resultados)
                    .FirstOrDefault(c => c.Codigo == codigoCampeonato);

                if (campeonato == null)
                {
                    return new GenericResponse<Repository.Tables.Campeonato>
                    {
                        mensagem = "not found"
                    };
                }

                return new GenericResponse<Repository.Tables.Campeonato>
                {
                    mensagem = "success",
                    response = campeonato
                };
            }
            catch (Exception ex)
            {
                return new GenericResponse<Repository.Tables.Campeonato>
                {
                    mensagem = "failed",
                    error = ex
                };
            }
        }

        public GenericResponse<List<Repository.Tables.Campeonato>> ObterCampeonatos()
        {
            try
            {
                var campeonatos = _context.Campeonatos
                    .Include(c => c.Fases)
                    .Include(c => c.Jogos)
                    .Include(c => c.Resultados)
                    .ToList();

                return new GenericResponse<List<Repository.Tables.Campeonato>>
                {
                    mensagem = "success",
                    response = campeonatos
                };
            }
            catch (Exception ex)
            {
                return new GenericResponse<List<Repository.Tables.Campeonato>>
                {
                    mensagem = "failed",
                    error = ex
                };
            }
        }
    }
}
