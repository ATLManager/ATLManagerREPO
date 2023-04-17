using ATLManager.Areas.Identity.Data;
using ATLManager.Models;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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
		public IFormFile? ProfilePicture { get; set; }

        [Display(Name = "Caminho do ficheiro PDF")]
        public IFormFile? DeclaracaoMedica { get; set; }

        [Display(Name = "Caminho do ficheiro PDF")]
        public IFormFile? BoletimVacinas { get; set; }
    }
}
