﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApiGintec.Repository.Tables
{
    public class Usuario
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Codigo { get; set; }

        [Required]
        [StringLength(150)]
        public string Nome { get; set; }

        [Required]
        [StringLength(30)]
        public string RM { get; set; }

        [Required]
        [StringLength(150)]
        public string Email { get; set; }

        [Required]
        public int Status { get; set; }

        [Required]
        [StringLength(100)]
        public string Senha { get; set; }
        
        public int? SalaCodigo { get; set; }

        [Required]
        public bool isPadrinho { get; set; }
        public string? fotoPerfil { get; set; }
        public int? oficinacodigo { get; set; }

        [ForeignKey("SalaCodigo")]
        public Sala? Sala { get; set; }
        [ForeignKey("oficinacodigo")]
        public Oficina? Oficina { get; set; }

        [ForeignKey("Status")]
        public StatusUsuario StatusUsuario { get; set; }

        public int? AtividadeCodigo { get; set; }

        [ForeignKey("AtividadeCodigo")]
        public Atividade? Atividade { get; set; }
        
        public int? CampeonatoCodigo { get; set; }

        [ForeignKey("CampeonatoCodigo")]
        public Campeonato? Campeonato { get; set; }
        public List<AtividadeCampeonatoRealizada>? AtividadeCampeonatoRealizada { get; set; }
    }
}
