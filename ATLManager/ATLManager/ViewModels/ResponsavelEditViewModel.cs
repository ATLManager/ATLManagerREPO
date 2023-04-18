using ATLManager.Areas.Identity.Data;
using ATLManager.Models;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ATLManager.ViewModels
{
    public class ResponsavelEditViewModel
    {
        public Guid EducandoResponsavelId { get; set; }

        public Guid EducandoId { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        [DataType(DataType.Text)]
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
        public IFormFile? ProfilePicture { get; set; }
	}
}
