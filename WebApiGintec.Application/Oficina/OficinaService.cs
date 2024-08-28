using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApiGintec.Application.Campeonato.Models;
using WebApiGintec.Application.Util;
using WebApiGintec.Repository;

namespace WebApiGintec.Application.Oficina
{
    public class OficinaService
    {
        private GintecContext _context;
        public OficinaService(GintecContext context)
        {
            _context = context;
        }

        public GenericResponse<List<OficinaFeitasResponse>> ObterOficinas(int usuarioCodigo)
        {
            try
            {
                var lstOficina = _context.Oficina.Include(x => x.HorariosFuncionamento).ToList();

                var lstResponse = new List<OficinaFeitasResponse>();
                var oficinasfeitas = _context.AtividadesCampeonatoRealizadas.Where(x => x.OficinaCodigo != null && x.UsuarioCodigo == usuarioCodigo).ToList();
                foreach (var oficina in lstOficina)
                {
                    lstResponse.Add(new OficinaFeitasResponse()
                    {
                        Codigo =oficina.Codigo,
                        Data = oficina.Data,
                        Descricao = oficina.Descricao,
                        HorariosFuncionamento = oficina.HorariosFuncionamento,                        
                        SalaCodigo = oficina.SalaCodigo,
                        isRealizada = oficinasfeitas.Any(x => x.OficinaHorario.OficinaCodigo  == oficina.Codigo),
                        OficinaHorarioFuncionamentoCodigo = oficinasfeitas.FirstOrDefault(x => x.OficinaHorario.OficinaCodigo == oficina.Codigo)?.Codigo,
                    });
                }
                return new GenericResponse<List<OficinaFeitasResponse>>()
                {
                    response = lstResponse,
                    mensagem = "success"
                };
            }
            catch (Exception ex)
            {
                return new GenericResponse<List<OficinaFeitasResponse>>()
                {
                    error = ex,
                    mensagem = "failed"
                };
            }
        }
    }
}
