﻿using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using Xunit.Sdk;

namespace ATLManager.Models
{
    public class Atividade
    {
        [Key]
        public Guid AtividadeId { get; set; }

        [Required]
        [Display(Name = "Nome")]
        [Column(TypeName = "nvarchar(100)")]
        public string Name { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        [Display(Name = "Data de Emissão")]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy HH:mm}", ApplyFormatInEditMode = true)]
        public DateTime StartDate { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        [Display(Name = "Data Limite")]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy HH:mm}", ApplyFormatInEditMode = true)]
        public DateTime EndDate { get; set; }

        [Required]
        [StringLength(255)]
        [DisplayName("Descrição")]
        public string Description { get; set; }

        [DisplayName("Fotografia")]
        public string Picture { get; set; }

        [ForeignKey("Atl")]
        public Guid? AtlId { get; set; }

        public Atividade()
        {
            AtividadeId = Guid.NewGuid();
        }
    }
}
