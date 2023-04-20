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

        public string FullName { get; set; }

        [Phone]
        [Required]
        [RegularExpression("^9[0-9]{8}$")]
        public string Phone { get; set; }

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
		[RegularExpression("^[0-9]*$", ErrorMessage = "Este campo deve conter apenas dígitos")]
		public int NIF { get; set; }

        [Required]
        [DisplayName("Fotografia de perfil")]
        public string ProfilePicture { get; set; }

        public EncarregadoEducacao()
		{
			EncarregadoId = Guid.NewGuid();
		}

		public EncarregadoEducacao(ATLManagerUser user, string firstName, string lastName, string phone, string address, string city, string postalCode, int nif) : this ()
        {
            User = user;
            UserId = user.Id;
            FullName = firstName + " " + lastName;
            Phone = phone;
            Address = address;
            City = city;
            PostalCode = postalCode;
            NIF = nif;
        }
    }
}
