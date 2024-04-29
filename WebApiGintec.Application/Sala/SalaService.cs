using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApiGintec.Application.Sala.Models;
using WebApiGintec.Application.Util;
using WebApiGintec.Repository;
using WebApiGintec.Repository.Tables;
using ZstdSharp.Unsafe;

namespace WebApiGintec.Application.Sala
{
    public class SalaService
    {
        private readonly GintecContext _context;
        public SalaService(GintecContext context)
        {
            _context = context;
        }
        public GenericResponse<List<Repository.Tables.Sala>> ObterTodasAsSalas()
        {
            try
            {
                var salasDb = _context.Salas.ToList();

                return new GenericResponse<List<Repository.Tables.Sala>>()
                {
                    response = salasDb,
                    mensagem = "success"
                };

            }
            catch (Exception ex)
            {
                return new GenericResponse<List<Repository.Tables.Sala>>()
                {
                    error = ex,
                    mensagem = "failed",
                    response = null
                };
            }
        }
        public GenericResponse<List<RankingSala>> ObterRanking()
        {
            try
            {
                var lstRank = new List<RankingSala>();
                var lstPontAlunos = _context.AtividadesCampeonatoRealizadas.Include(x => x.AtividadePontuacaoExtra).Include(x => x.Usuario).ToList();

                foreach (var item in _context.Salas.ToList())
                {
                    var salaPontos = lstPontAlunos.Where(x => x.Usuario.SalaCodigo == item.Codigo).ToList();
                    int pontosExtra = salaPontos.Where(x => x.AtividadePontuacaoExtraCodigo != null).Select(y => y.AtividadePontuacaoExtra.Pontuacao).Sum(u => u);
                    lstRank.Add(new RankingSala()
                    {
                        Codigo = item.Codigo,
                        Descricao = item.Descricao,
                        Pontuacao = salaPontos.Select(y => y.Codigo).Count() * 300 + pontosExtra
                    });
                }
                return new GenericResponse<List<RankingSala>>()
                {
                    response = lstRank,
                    mensagem = "success"
                };

            }
            catch (Exception ex)
            {
                return new GenericResponse<List<RankingSala>>()
                {
                    error = ex,
                    mensagem = "failed",
                    response = null
                };
            }
        }
        public GenericResponse<List<RankingSala>> ObterRanking(RankingRequest request)
        {
            try
            {
                var lstRank = new List<RankingSala>();
                var lstPontAlunos = _context.AtividadesCampeonatoRealizadas.Where(n => n.dataCad.Day == DateTime.Now.Day && n.dataCad.Month == DateTime.Now.Month && n.dataCad.Year == DateTime.Now.Year).Include(x => x.AtividadePontuacaoExtra).Include(x => x.Usuario).ToList();

                foreach (var item in _context.Salas.ToList())
                {
                    var salaPontos = lstPontAlunos.Where(x => x.Usuario.SalaCodigo == item.Codigo && x.Usuario.isPadrinho == request.IsPadrinho).ToList();
                    int pontosExtra = salaPontos.Where(x => x.AtividadePontuacaoExtraCodigo != null).Select(y => y.AtividadePontuacaoExtra.Pontuacao).Sum(u => u);
                    lstRank.Add(new RankingSala()
                    {
                        Codigo = item.Codigo,
                        Descricao = item.Descricao,
                        Pontuacao = salaPontos.Select(y => y.Codigo).Count() * 300 + pontosExtra
                    });
                }
                return new GenericResponse<List<RankingSala>>()
                {
                    response = lstRank,
                    mensagem = "success"
                };

            }
            catch (Exception ex)
            {
                return new GenericResponse<List<RankingSala>>()
                {
                    error = ex,
                    mensagem = "failed",
                    response = null
                };
            }
        }
    }
}
