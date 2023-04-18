using ATLManager.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ATLManager.Models
{
    public class EducandoResponsavel
    {
        [Key]
        public Guid EducandoResponsavelId { get; set; }

        [ForeignKey("Educando")]
        public Guid EducandoId { get; set; }
        public Educando? Educando { get; set; }

		[Required]
		[Column(TypeName = "nvarchar(100)")]
		public string Name { get; set; }

		[Required]
		[Column(TypeName = "nvarchar(100)")]
		public string Apelido { get; set; }

		[Required]
		[StringLength(9, MinimumLength = 9, ErrorMessage = "Este campo deve conter 9 dígitos")]
		[RegularExpression("^[0-9]*$", ErrorMessage = "Este campo deve conter apenas dígitos")]
		public string CC { get; set; }

		[Phone]
		[Required]
		[RegularExpression("^9[0-9]{8}$")]
		[DisplayName("Número de Telemóvel")]
		[StringLength(9, MinimumLength = 9, ErrorMessage = "Este campo deve conter 9 dígitos")]
		public string Phone { get; set; }

		[Required]
		[DisplayName("Fotografia do Responsável")]
		public string ProfilePicture { get; set; }

		[DisplayName("Grau de Parentesco (se aplicável)")]
		public string? Parentesco { get; set; }

		public EducandoResponsavel()
		{
			EducandoResponsavelId = Guid.NewGuid();
		}
    }
}
