using ATLManager.Areas.Identity.Data;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ATLManager.Models
{
    public class EncarregadoEducacao
    {
        [Key]
        public Guid EncarregadoId { get; set; }

        [ForeignKey("User")]
        public string UserId { get; set; }
		public ATLManagerUser User { get; set; }

        [Required]
        [DisplayName("Morada")]
        public string Address { get; set; }

        [Required]
        [DisplayName("Código Postal")]
        [RegularExpression(@"^\d{4}-\d{3}$", ErrorMessage = "Formato Incorreto - ex. 1234-123")]
        public string PostalCode { get; set; }
        
        [Required]
        [MaxLength(20)]
        [DisplayName("Cidade")]
        public string City { get; set; }

        [Required]
		[StringLength(9, MinimumLength = 9, ErrorMessage = "Este campo deve conter 9 dígitos")]
		[RegularExpression("^[0-9]*$", ErrorMessage = "Este campo deve conter apenas dígitos")]
		public string NIF { get; set; }

        public EncarregadoEducacao()
		{
			EncarregadoId = Guid.NewGuid();
		}

		public EncarregadoEducacao(string userId, string address, string city, string postalCode, string nif) : this ()
        {
            UserId = userId;
            Address = address;
            City = city;
            PostalCode = postalCode;
            NIF = nif;
        }
    }
}
