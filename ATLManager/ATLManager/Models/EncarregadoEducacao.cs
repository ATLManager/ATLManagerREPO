using ATLManager.Areas.Identity.Data;
using System.ComponentModel.DataAnnotations;

namespace ATLManager.Models
{
    public class EncarregadoEducacao
    {
        [Key]
        public Guid EncarregadoID { get; set; }

		[Required]
		public ATLManagerUser User { get; set; }

        [Required]
        [Phone]
        public int Phone { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 5)]
        public string Address { get; set; }

        [Required]
        [RegularExpression(@"^\d{4}-\d{3}$", ErrorMessage = "Formato Incorreto - ex. 1234-123")]
        public string PostalCode { get; set; }
        
        [Required]
        [MaxLength(20)]
        public string City { get; set; }

        [Required]
        [StringLength(9, MinimumLength = 9, ErrorMessage = "Este campo deve conter 9 dígitos")]
        public int NIF { get; set; }



		public EncarregadoEducacao()
		{
			EncarregadoID = Guid.NewGuid();
		}

		public EncarregadoEducacao(ATLManagerUser user, int phone, string address, string city, string postalCode, int nif) : this ()
        {
            User = user;
            Phone = phone;
            Address = address;
            City = city;
            PostalCode = postalCode;
            NIF = nif;
        }
    }
}
