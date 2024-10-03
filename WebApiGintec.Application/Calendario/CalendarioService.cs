using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
                var lstCalendar = _context.Calendario.Include(x => x.Campeonatos).Where(u => u.DataGincana.Date >= DateTime.Now.Date).ToList();
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
    }
}
