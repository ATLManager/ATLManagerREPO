using System.ComponentModel.DataAnnotations;

namespace ATLManager.Models
{
    public class EncarregadoEducacao
    {
        [Key]
        public Guid EncarregadoID { get; set; }

        [Required]
        [MaxLength(20)]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(20)]
        public string LastName { get; set; }

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

        public EncarregadoEducacao(string firstName, string lastName, int phone, 
            string address, string postalCode, string city, int nif)
        {
            FirstName = firstName;
            LastName = lastName;
            Phone = phone;
            Address = address;
            PostalCode = postalCode;
            City = city;
            NIF = nif;
        }
    }
}
