using ATLManager.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace ATLManager.Models
{
    public class ATL
    {
        [Key]
        public Guid AtlId { get; set; }

		[Required]
		public string Name { get; set; }

		[Required]
        public string Address { get; set; }
        
        [Required]
		public string City { get; set; }

        [Required]
        [RegularExpression(@"^\d{4}-\d{3}$", ErrorMessage = "Formato Incorreto - ex. 1234-123")]
        public string PostalCode { get; set; }

        [ForeignKey("Agrupamento")]
        public Guid? AgrupamentoId { get; set; }

		public Agrupamento? Agrupamento { get; set; }

		[StringLength(9, MinimumLength = 9, ErrorMessage = "Este campo deve conter 9 dígitos")]
		[RegularExpression("^[0-9]*$", ErrorMessage = "Este campo deve conter apenas dígitos")]
		public string? NIPC { get; set; }

		[Required]
		[DisplayName("Logo do Agrupamento")]
		public string LogoPicture { get; set; }

		public ATL()
		{
			AtlId = Guid.NewGuid();
		}

		public ATL(string name, string address, string city, string postalCode, string nipc) : this ()
        {
            Name = name;
            Address = address;
            City = city;
            PostalCode = postalCode;
            NIPC = nipc;
        }
    }
}
