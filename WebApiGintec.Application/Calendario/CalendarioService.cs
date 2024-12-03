using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApiGintec.Application.Calendario.Models;
using WebApiGintec.Application.Util;
using WebApiGintec.Repository;
using WebApiGintec.Repository.Tables;

namespace WebApiGintec.Application.Calendario
{
    public class CalendarioService
    {
        private readonly GintecContext _context;
        public CalendarioService(GintecContext context)
        {
            _context = context;
        }
        public GenericResponse<List<Repository.Tables.Calendario>> ObterDatas()
        {
            try
            {
                var lstCalendar = _context.Calendario.Include(x => x.Campeonatos.Where(x => x.isAtivo == true)).Where(u => u.DataGincana.Date >= DateTime.Now.Date && u.isDeleted == false).ToList();
                return new GenericResponse<List<Repository.Tables.Calendario>>()
                {
                    mensagem = "success",
                    response = lstCalendar
                };
            }
            catch (Exception ex)
            {
                return new GenericResponse<List<Repository.Tables.Calendario>>()
                {
                    error = ex,
                    mensagem = "failed",
                    response = null
                };
            }
        }
        public GenericResponse<List<Repository.Tables.Calendario>> ObterDatasAdmin()
        {
            try
            {
                var lstCalendar = _context.Calendario.Include(x => x.Campeonatos.Where(x => x.isAtivo == true)).Where(p => p.isDeleted == false).ToList();
                return new GenericResponse<List<Repository.Tables.Calendario>>()
                {
                    mensagem = "success",
                    response = lstCalendar
                };
            }
            catch (Exception ex)
            {
                return new GenericResponse<List<Repository.Tables.Calendario>>()
                {
                    error = ex,
                    mensagem = "failed",
                    response = null
                };
            }
        }
        public GenericResponse<Repository.Tables.Calendario> AdicionarCalendario(CalendarioRequest request)
        {
            try
            {
                var novoCalendario = new Repository.Tables.Calendario
                {
                    DataGincana = request.DataGincana                    
                };

                var calendarioDb = _context.Calendario.Add(novoCalendario);
                _context.SaveChanges();

                return new GenericResponse<Repository.Tables.Calendario>
                {
                    mensagem = "success",
                    response = calendarioDb.Entity
                };
            }
            catch (Exception ex)
            {
                return new GenericResponse<Repository.Tables.Calendario>
                {
                    mensagem = "failed",
                    error = ex,
                    response = null
                };
            }
        }
        public GenericResponse<Repository.Tables.Calendario> AtualizarCalendario(int calendarioCodigo, CalendarioRequest request)
        {
            try
            {
                var calendario = _context.Calendario.FirstOrDefault(c => c.Codigo == calendarioCodigo);
                if (calendario == null)
                {
                    return new GenericResponse<Repository.Tables.Calendario>
                    {
                        mensagem = "Calendário não encontrado",
                        response = null
                    };
                }

                calendario.DataGincana = request.DataGincana;                

                _context.Calendario.Update(calendario);
                _context.SaveChanges();

                return new GenericResponse<Repository.Tables.Calendario>
                {
                    mensagem = "success",
                    response = calendario
                };
            }
            catch (Exception ex)
            {
                return new GenericResponse<Repository.Tables.Calendario>
                {
                    mensagem = "failed",
                    error = ex,
                    response = null
                };
            }
        }
        public GenericResponse<bool> DeletarCalendario(int calendarioCodigo)
        {
            try
            {
                var calendario = _context.Calendario.FirstOrDefault(c => c.Codigo == calendarioCodigo);
                if (calendario == null)
                {
                    return new GenericResponse<bool>
                    {
                        mensagem = "Calendário não encontrado",
                        response = false
                    };
                }
                calendario.isDeleted = true;
                _context.Calendario.Update(calendario);
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



    }
}
