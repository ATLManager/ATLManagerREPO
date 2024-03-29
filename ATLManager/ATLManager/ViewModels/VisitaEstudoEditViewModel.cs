﻿using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using ATLManager.Models;
using ATLManager.Attributes;
using Xunit.Sdk;

namespace ATLManager.ViewModels
{
    public class VisitaEstudoEditViewModel
    {
        public Guid VisitaEstudoID { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "Máximo 50 caratéres")]
        public string Name { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "Máximo 50 caratéres")]
        public string Location { get; set; }

        [Required]
        [StringLength(255)]
        public string Descripton { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Data")]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? Date { get; set; }

        [DataType(DataType.Upload)]
        [DisplayName("Local da Visita de Estudo")]
        [AllowedExtensions(new string[] { ".jpg", ".jpeg", ".png" },
            ErrorMessage = "A extensão do ficheiro escolhido não é permitida: .jpg, .jpeg, .png")]
		[MaxFileSize(128 * 1024, ErrorMessage = "Tamanho máximo permitido é de 128kB")]
		public IFormFile? Picture { get; set; }

        public VisitaEstudoEditViewModel()
        {
        }

        public VisitaEstudoEditViewModel(VisitaEstudo visitaEstudo)
        {
            VisitaEstudoID = visitaEstudo.VisitaEstudoID;
            Name = visitaEstudo.Name;
            Location = visitaEstudo.Location;
            Descripton = visitaEstudo.Description;
            Date = visitaEstudo.Date;
        }
    }
}
