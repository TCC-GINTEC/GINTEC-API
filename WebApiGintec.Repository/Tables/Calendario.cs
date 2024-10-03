using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApiGintec.Repository.Tables
{    
    public class Calendario
    {
        [Key]        
        public int Codigo { get; set; }

        [Required]        
        public DateTime DataGincana { get; set; }
        [Required]        
        public bool isAtivo { get; set; }
        
        public List<Campeonato> Campeonatos { get; set; }
    }

}
