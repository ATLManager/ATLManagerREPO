﻿using ATLManager.Areas.Identity.Data;
using ATLManager.Attributes;
using ATLManager.Models;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ATLManager.ViewModels
{
    public class AgrupamentoEditViewModel
    {
		public Guid AgrupamentoId { get; set; }

		[Required]
		[DisplayName("Nome")]
        [StringLength(50, ErrorMessage = "Máximo 50 caratéres")]
        public string Name { get; set; }

		[Required]
		[MaxLength(20)]
		[DisplayName("Localização")]
        [StringLength(50, ErrorMessage = "Máximo 50 caratéres")]
        public string Location { get; set; }

		[Required]
		[StringLength(9, MinimumLength = 9, ErrorMessage = "Este campo deve conter 9 dígitos")]
		[RegularExpression("^[0-9]*$", ErrorMessage = "Este campo deve conter apenas dígitos")]
		public string NIPC { get; set; }

		[DataType(DataType.Upload)]
		[DisplayName("Logo do Agrupamento")]
        [AllowedExtensions(new string[] { ".jpg", ".jpeg", ".png" }, 
			ErrorMessage = "A extensão do ficheiro escolhido não é permitida: .jpg, .jpeg, .png")]
		[MaxFileSize(128 * 1024, ErrorMessage = "Tamanho máximo permitido é de 128kB")]
		public IFormFile? LogoPicture { get; set; }

		public AgrupamentoEditViewModel ()
		{
		}

		public AgrupamentoEditViewModel(Agrupamento agrupamento) {
			AgrupamentoId = agrupamento.AgrupamentoID;
			Name = agrupamento.Name;
			Location = agrupamento.Location;
			NIPC = agrupamento.NIPC;
		}
	}
}
