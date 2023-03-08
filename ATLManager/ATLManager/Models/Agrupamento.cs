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

		[Required]
		[StringLength(9, MinimumLength = 9, ErrorMessage = "Este campo deve conter 9 dígitos")]
		[RegularExpression("^[0-9]*$", ErrorMessage = "Este campo deve conter apenas dígitos")]
		[Display(Name = "Certidão Permanente")]
		public string NIPC { get; set; }

		public Agrupamento()
        {
            AgrupamentoID = Guid.NewGuid();
        }

        public Agrupamento(string name, string location) : this ()
        {
            Name = name;
            Location = location;
        }
    }
}
