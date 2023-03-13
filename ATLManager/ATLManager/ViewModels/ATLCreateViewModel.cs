using ATLManager.Areas.Identity.Data;
using ATLManager.Models;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ATLManager.ViewModels
{
    public class ATLCreateViewModel
    {
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

		[ForeignKey("Agrupamento")]
		public Guid? AgrupamentoId { get; set; }

		[StringLength(9, MinimumLength = 9, ErrorMessage = "Este campo deve conter 9 dígitos")]
		[RegularExpression("^[0-9]*$", ErrorMessage = "Este campo deve conter apenas dígitos")]
		public string? NIPC { get; set; }

		[Required]
		[DisplayName("Logo do Agrupamento")]
		public IFormFile LogoPicture { get; set; }
	}
}
