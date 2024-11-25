using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApiGintec.Repository.Tables
{
    public class PermissaoStatus
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int codigo { get; set; }
        public  int statuscodigo { get; set; }
        public string pathScreen { get; set; }
        public string nome { get; set; }
        public string icon { get; set; }
    }
}
