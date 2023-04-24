﻿using ATLManager.Areas.Identity.Data;
using ATLManager.Attributes;
using ATLManager.Models;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Xunit.Sdk;

namespace ATLManager.ViewModels
{
    public class ATLEditViewModel
    {
		public Guid AtlId { get; set; }

		[Required]
		[DisplayName("Nome")]
		public string Name { get; set; }

		[Required]
		[StringLength(50, MinimumLength = 5)]
		public string Address { get; set; }

		[Required]
		[StringLength(20, MinimumLength = 5)]
		public string City { get; set; }

		[Required]
		[RegularExpression(@"^\d{4}-\d{3}$", ErrorMessage = "Formato Incorreto - ex. 1234-123")]
		public string PostalCode { get; set; }

		public Guid? AgrupamentoId { get; set; }

		[StringLength(9, MinimumLength = 9, ErrorMessage = "Este campo deve conter 9 dígitos")]
		[RegularExpression("^[0-9]*$", ErrorMessage = "Este campo deve conter apenas dígitos")]
		public string? NIPC { get; set; }

        [DataType(DataType.Upload)]
        [DisplayName("Logo do Agrupamento")]
        [AllowedExtensions(new string[] { ".jpg", ".jpeg", ".png" },
            ErrorMessage = "A extensão do ficheiro escolhido não é permitida: .jpg, .jpeg, .png")]
        public IFormFile? LogoPicture { get; set; }

		public ATLEditViewModel()
		{
		}

        public ATLEditViewModel(ATL atl)
        {
            AtlId = atl.AtlId;
            Name = atl.Name;
            Address = atl.Address;
            City = atl.City;
            PostalCode = atl.PostalCode;
            NIPC = atl.NIPC;
        }
    }
}
