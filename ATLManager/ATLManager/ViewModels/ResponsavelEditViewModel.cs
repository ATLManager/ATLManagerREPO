using ATLManager.Areas.Identity.Data;
using ATLManager.Attributes;
using ATLManager.Models;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Xunit.Sdk;

namespace ATLManager.ViewModels
{
    public class ResponsavelEditViewModel
    {
        public Guid EducandoResponsavelId { get; set; }

        public Guid EducandoId { get; set; }

        [Required]
        [StringLength(20, ErrorMessage = "Máximo 20 caratéres")]
        public string Name { get; set; }

        [Required]
        [StringLength(20, ErrorMessage = "Máximo 20 caratéres")]
        public string Apelido { get; set; }

        [Required]
        [DisplayName("Cartão de cidadão")]
        [StringLength(9, MinimumLength = 9, ErrorMessage = "Este campo deve conter 9 dígitos")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Este campo deve conter apenas dígitos")]
        public string CC { get; set; }

		[Phone]
		[Required]
		[RegularExpression("^9[0-9]{8}$")]
		[DisplayName("Número de Telemóvel")]
        [StringLength(9, MinimumLength = 9, ErrorMessage = "Este campo deve conter 9 dígitos")]
		public string Phone { get; set; }

		[DisplayName("Grau de Parentesco (se aplicável)")]
		public string? Parentesco { get; set; }

		[DisplayName("Imagem de perfil")]
        [AllowedExtensions(new string[] { ".jpg", ".jpeg", ".png" },
            ErrorMessage = "A extensão do ficheiro escolhido não é permitida: .jpg, .jpeg, .png")]
		[MaxFileSize(128 * 1024, ErrorMessage = "Tamanho máximo permitido é de 128kB")]
		public IFormFile? ProfilePicture { get; set; }
	}
}
