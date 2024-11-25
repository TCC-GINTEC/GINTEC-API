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
        public GenericResponse<List<ParticipacaoSalas>> ObterParticipacaoSalas()
        {
            try
            {
                var salas = _context.Salas.Where(x => x.Descricao != "Não possui sala").ToList();

                var lstPontuacao = new List<ParticipacaoSalas>();

                var lstusuariocodigo = _context.Usuarios.ToList();

                var atts = _context.AtividadesCampeonatoRealizadas.ToList();

                foreach (var item in salas)
                {
                    var lstcodigousers = lstusuariocodigo.Where(x => x.SalaCodigo == item.Codigo).Select(y => y.Codigo);
                    lstPontuacao.Add(new ParticipacaoSalas()
                    {
                        SalaNome = $"{item.Serie}º {item.Descricao}",
                        Pontuacao = atts.Where(x => lstcodigousers.Contains(x.UsuarioCodigo)).ToList().Count()
                    });
                }
                var porcentagens = new List<ParticipacaoSalas>();
                var pontuacaoPrimeiroLugar = lstPontuacao.OrderByDescending(x => x.Pontuacao).First().Pontuacao;

                foreach (var item in lstPontuacao.OrderByDescending(x => x.Pontuacao))
                {
                    int porcentagem = (int)Math.Ceiling((decimal)item.Pontuacao / pontuacaoPrimeiroLugar * 100);
                    porcentagens.Add(new ParticipacaoSalas()
                    {
                        SalaNome = item.SalaNome,
                        Pontuacao = porcentagem
                    });
                }

                return new GenericResponse<List<ParticipacaoSalas>>()
                {
                    mensagem = "success",
                    response = porcentagens
                };
            }
            catch (Exception ex)
            {
                return new GenericResponse<List<ParticipacaoSalas>>()
                {
                    error = ex,
                    mensagem = "failed",
                    response = null
                };
            }
        }

        public GenericResponse<List<Repository.Tables.Sala>> ObterTodasAsSalas()
        {
            try
            {
                var salasDb = _context.Salas.ToList();

                foreach (var sala in salasDb)
                {
                    if (!string.IsNullOrEmpty(sala.FotoSala))
                    {
                        sala.FotoSala = new S3Service("salafoto").GetUrlFile(sala.FotoSala, 6);
                    }
                }
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
        public GenericResponse<Repository.Tables.Sala> ObterSalaPorCodigo(int codigo)
        {
            try
            {
                var salaDb = _context.Salas.FirstOrDefault(x => x.Codigo == codigo);
                if (!string.IsNullOrEmpty(salaDb.FotoSala))
                {
                    salaDb.FotoSala = new S3Service("salafoto").GetUrlFile(salaDb.FotoSala, 6);
                }

                return new GenericResponse<Repository.Tables.Sala>()
                {
                    response = salaDb,
                    mensagem = "success"
                };

            }
            catch (Exception ex)
            {
                return new GenericResponse<Repository.Tables.Sala>()
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

                var lstPontAlunosAtt = (from acr in _context.AtividadesCampeonatoRealizadas
                                        join ap in _context.AtividadesPontuacaoExtra on acr.AtividadePontuacaoExtraCodigo equals ap.Codigo into ap_join
                                        from ap in ap_join.DefaultIfEmpty()
                                        join u in _context.Usuarios on acr.UsuarioCodigo equals u.Codigo
                                        join a in _context.Atividades on acr.AtividadeCodigo equals a.Codigo
                                        join c in _context.Calendario on a.CalendarioCodigo equals c.Codigo
                                        where c.isAtivo == true
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
                                          where c.isAtivo == true
                                          select new
                                          {
                                              Campeonato = camp,
                                              Fase = fase,
                                              Usuario = u,
                                              Calendario = c
                                          }).ToList();

                var pontosExtrasQVemDoNada = _context.AlunoPontuacao.Include(x => x.Usuario).ToList(); ;

                foreach (var sala in _context.Salas.ToList())
                {
                    if (sala.Descricao == "Não possui sala")
                        continue;
                    var salaPontos = lstPontAlunosAtt.Where(x => x.Usuario.SalaCodigo == sala.Codigo).ToList();

                    int pontosExtra = salaPontos.Where(x => x.AtividadePontuacaoExtra != null)
                                                .Select(y => y.AtividadePontuacaoExtra.Pontuacao)
                                                .Sum();

                    var pontosextraQVemDOnADAdAsALA = pontosExtrasQVemDoNada.Where(x => x.Usuario.SalaCodigo == sala.Codigo).Sum(u => u.Pontos);


                    int pontuacaoAtividades = salaPontos.Count * 600;

                    var salaFases = lstPontAlunosChamp.Where(x => x.Usuario.SalaCodigo == sala.Codigo).ToList();

                    int pontuacaoFases = salaFases.Count * 350;

                    // Verificando e obtendo a URL da foto da sala, caso exista
                    if (!string.IsNullOrEmpty(sala.FotoSala))
                    {
                        sala.FotoSala = new S3Service("salafoto").GetUrlFile(sala.FotoSala, 6);
                    }

                    lstRank.Add(new RankingSala()
                    {
                        Codigo = sala.Codigo,
                        Descricao = $"{sala.Serie}° {sala.Descricao}",
                        FotoSala = sala.FotoSala,
                        Pontuacao = pontuacaoAtividades + pontuacaoFases + pontosExtra + pontosextraQVemDOnADAdAsALA // Soma das pontuações
                    });
                }

                // Retornando a resposta com o ranking ordenado pela pontuação
                return new GenericResponse<List<RankingSala>>()
                {
                    response = lstRank.OrderByDescending(x => x.Pontuacao).ToList(),
                    mensagem = "success"
                };
            }
            catch (Exception ex)
            {
                // Captura de exceção para retorno de erro
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
                var lstPontAlunos = _context.AtividadesCampeonatoRealizadas.Include(x => x.AtividadePontuacaoExtra).Include(x => x.Usuario).ToList().Where(n => n.dataCad.ToString("dd/MM/yyyy") == request.FiltroData.ToString("dd/MM/yyyy")).ToList();

                foreach (var item in _context.Salas.ToList())
                {
                    var salaPontos = lstPontAlunos.Where(x => x.Usuario.SalaCodigo == item.Codigo && x.Usuario.isPadrinho == request.IsPadrinho).ToList();
                    int pontosExtra = salaPontos.Where(x => x.AtividadePontuacaoExtraCodigo != null).Select(y => y.AtividadePontuacaoExtra.Pontuacao).Sum(u => u);
                    if (!string.IsNullOrEmpty(item.FotoSala))
                    {
                        item.FotoSala = new S3Service("salafoto").GetUrlFile(item.FotoSala, 6);
                    }
                    lstRank.Add(new RankingSala()
                    {
                        Codigo = item.Codigo,
                        Descricao = $"{item.Serie}° {item.Descricao}",
                        FotoSala = item.FotoSala,
                        Pontuacao = salaPontos.Select(y => y.Codigo).Count() * 600 + pontosExtra
                    });
                }
                return new GenericResponse<List<RankingSala>>()
                {
                    response = lstRank.OrderByDescending(x => x.Pontuacao).ToList(),
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
