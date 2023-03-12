using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ATLManager.Models
{
    public class Agrupamento
    {
        [Key]
        public Guid AgrupamentoID { get; set; }

		[Required]
        [MaxLength(20)]
        public string Name { get; set; }

        [Required]
        [MaxLength(20)]
        public string Location { get; set; }

		[StringLength(9, MinimumLength = 9, ErrorMessage = "Este campo deve conter 9 dígitos")]
		[RegularExpression("^[0-9]*$", ErrorMessage = "Este campo deve conter apenas dígitos")]
		public string NIPC { get; set; }

		[Required]
		[DisplayName("Logo do Agrupamento")]
		public string LogoPicture { get; set; }

		public Agrupamento()
        {
            AgrupamentoID = Guid.NewGuid();
        }

        public Agrupamento(string name, string location, string nipc) : this ()
        {
            Name = name;
            Location = location;
            NIPC = nipc;
        }
    }
}
