﻿using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ATLManager.Models;

namespace ATLManager.Models
{
    public class Formulario
    {
        [Key]
        public Guid FormularioId { get; set; }

		[Required]
        [DisplayName("Nome")]
        public string Name { get; set; }

        [Required]
        [MaxLength(1000)]
        [DisplayName("Descrição")]
        public string Description { get; set; }

        [ForeignKey("VisitaEstudo")]
        [DisplayName("Visita de Estudo")]
        public Guid? VisitaEstudoId { get; set; }
        [DisplayName("Visita de Estudo")]
        public VisitaEstudo? VisitaEstudo { get; set; }

        [ForeignKey("Atividade")]
        [DisplayName("Atividade")]
        public Guid? AtividadeId { get; set; }
        [DisplayName("Atividade")]
        public Atividade? Atividade { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [DisplayName("Data de Emissão")]
        [DisplayFormat(DataFormatString = "{0:MM-dd-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime StartDate { get; set; } = DateTime.UtcNow;

        [Required]
        [DataType(DataType.Date)]
        [DisplayName("Data Limite")]
        [DisplayFormat(DataFormatString = "{0:MM-dd-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime DateLimit { get; set; }

        [ForeignKey("Atl")]
        public Guid? AtlId { get; set; }
        public ATL? Atl { get; set; }

        public Formulario()
        {
            FormularioId = Guid.NewGuid();
        }
    }
}
