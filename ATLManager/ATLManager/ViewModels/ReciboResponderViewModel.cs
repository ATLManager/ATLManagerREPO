﻿using ATLManager.Areas.Identity.Data;
using ATLManager.Attributes;
using ATLManager.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Xunit.Sdk;

namespace ATLManager.ViewModels
{
    public class ReciboResponderViewModel
    {
		public Guid ReciboRespostaId { get; set; }

		[DisplayName("Nome do formulário")]
		public string Name { get; set; }

		[DisplayName("Educando")]
		public string Educando { get; set; }
		
		[DisplayName("NIB")]
		public string NIB { get; set; }

		[DisplayName("Preço")]
		[RegularExpression("[1-9][0-9]*?[,.][0-9]{0,2}")]
		public string Price { get; set; }

		[DisplayName("Descrição")]
		public string Description { get; set; }

		[DataType(DataType.Date)]
		[DisplayName("Data Limite")]
		[DisplayFormat(DataFormatString = "{0:MM-dd-yyyy}", ApplyFormatInEditMode = true)]
		public string DateLimit { get; set; }

        [DisplayName("Comprovativo")]
        [AllowedExtensions(new string[] { ".pdf" },
            ErrorMessage = "A extensão do ficheiro escolhido não é permitida: .pdf")]
		[MaxFileSize(3 * 1024 * 1024, ErrorMessage = "Tamanho máximo permitido é de 3mB")]
		public IFormFile Comprovativo { get; set; }
	}
}
