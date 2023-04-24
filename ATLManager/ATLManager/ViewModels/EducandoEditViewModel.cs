using ATLManager.Areas.Identity.Data;
using ATLManager.Attributes;
using ATLManager.Models;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Xunit.Sdk;

namespace ATLManager.ViewModels
{
    public class EducandoEditViewModel
    {
		public Guid EducandoId { get; set; }

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
		public Guid AtlId { get; set; }
		public Guid EncarregadoId { get; set; }

        [DataType(DataType.Upload)]
        [DisplayName("Imagem da criança")]
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

        public EducandoEditViewModel()
        {
        }

        public EducandoEditViewModel(Educando educando)
        {
            EducandoId = educando.EducandoId;
            Name = educando.Name;
            Apelido = educando.Apelido;
            CC = educando.CC;
            Genero = educando.Genero;
            AtlId = educando.AtlId;
            EncarregadoId = educando.EncarregadoId;
        }
    }
}
