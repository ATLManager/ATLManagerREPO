using ATLManager.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace ATLManager.Models
{
    public class ATL
    {
        [Key]
        public Guid AtlId { get; set; }

		[Required]
		[Column(TypeName = "nvarchar(100)")]
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

		public Agrupamento AgrupamentoPai { get; set; }

		[StringLength(9, MinimumLength = 9, ErrorMessage = "Este campo deve conter 9 dígitos")]
		[RegularExpression("^[0-9]*$", ErrorMessage = "Este campo deve conter apenas dígitos")]
		public string NIPC { get; set; }

		public ATL()
		{
			AtlId = Guid.NewGuid();
		}

		public ATL(string name, int phone, string address, string city, string postalCode, string nipc) : this ()
        {
            Name = name;
            Address = address;
            City = city;
            PostalCode = postalCode;
            NIPC = nipc;
        }
    }
}
