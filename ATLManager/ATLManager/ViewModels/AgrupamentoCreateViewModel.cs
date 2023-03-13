﻿using ATLManager.Areas.Identity.Data;
using ATLManager.Models;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ATLManager.ViewModels
{
    public class AgrupamentoCreateViewModel
    {
		[Required]
		[DisplayName("Nome")]
		public string Name { get; set; }

		[Required]
		[MaxLength(20)]
		[DisplayName("Localização")]
		public string Location { get; set; }

		[Required]
		[StringLength(9, MinimumLength = 9, ErrorMessage = "Este campo deve conter 9 dígitos")]
		[RegularExpression("^[0-9]*$", ErrorMessage = "Este campo deve conter apenas dígitos")]
		public string NIPC { get; set; }

		[Required]
		[DisplayName("Logo do Agrupamento")]
		public IFormFile LogoPicture { get; set; }
	}
}