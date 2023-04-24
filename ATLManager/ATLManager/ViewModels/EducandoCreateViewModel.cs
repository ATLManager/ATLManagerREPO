using ATLManager.Areas.Identity.Data;
using ATLManager.Models;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ATLManager.Attributes;
using Xunit.Sdk;

namespace ATLManager.ViewModels
{
    public class EducandoCreateViewModel
    {
		[Required]
		[Column(TypeName = "nvarchar(100)")]
		public string Name { get; set; }

		[Required]
		[Column(TypeName = "nvarchar(100)")]
		public string Apelido { get; set; }

		[Required]
		[StringLength(9, MinimumLength = 9, ErrorMessage = "Este campo deve conter 9 dígitos")]
		public string CC { get; set; }

		[Required]
		public string Genero { get; set; }
		public Guid EncarregadoId { get; set; }

		[DisplayName("Imagem da educando")]
        [AllowedExtensions(new string[] { ".jpg", ".jpeg", ".png" },
            ErrorMessage = "A extensão do ficheiro escolhido não é permitida: .jpg, .jpeg, .png")]
        public IFormFile? ProfilePicture { get; set; }

        [Display(Name = "Caminho do ficheiro PDF")]
        [AllowedExtensions(new string[] { ".pdf" },
            ErrorMessage = "A extensão do ficheiro escolhido não é permitida: .pdf")]
        public IFormFile? DeclaracaoMedica { get; set; }

        [Display(Name = "Caminho do ficheiro PDF")]
        [AllowedExtensions(new string[] { ".pdf" },
            ErrorMessage = "A extensão do ficheiro escolhido não é permitida: .pdf")]
        public IFormFile? BoletimVacinas { get; set; }

		[DataType(DataType.Date)]
        [DisplayName("Data de Nascimento")]
		[DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime BirthDate { get; set; }
    }
}
