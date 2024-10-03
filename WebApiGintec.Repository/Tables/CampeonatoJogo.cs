using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApiGintec.Repository.Tables
{
    public class CampeonatoJogo
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Codigo { get; set; }

        [Required]
        public int FaseCodigo { get; set; }

        [Required]
        public int Sala1Codigo { get; set; }

        [Required]
        public int Sala2Codigo { get; set; }

        [Required]
        public DateTime DataJogo { get; set; }
        public int? TimeCodigo1 { get; set; }  
        public int? TimeCodigo2 { get; set; }  

        [ForeignKey("Sala1Codigo")]
        public Sala Sala1 { get; set; }

        [ForeignKey("FaseCodigo")]
        public CampeonatoFase Fase { get; set; }

        [ForeignKey("Sala2Codigo")]
        public Sala Sala2 { get; set; }
        public List<CampeonatoResultado> Resultados { get; set; }        
    }
}
