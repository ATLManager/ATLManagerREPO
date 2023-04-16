using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ATLManager.Models
{
    public class Recibo
    {
        [Key]
        public Guid ReciboId { get; set; }

		[Required]
        [MaxLength(50)]
        [DisplayName("Nome")]
        public string Name { get; set; }

        [Required]
        [DisplayName("Preço")]
        [RegularExpression("[1-9][0-9]*?[,.][0-9]{0,2}")]
        public string Price { get; set; }

        [Required]
		[StringLength(21, MinimumLength = 21, ErrorMessage = "Este campo deve conter 21 dígitos")]
		[RegularExpression("^[0-9]*$", ErrorMessage = "Este campo deve conter apenas dígitos")]
		public string NIB { get; set; }

        [Required]
        [MaxLength(500)]
        [DisplayName("Descrição")]
        public string Description { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [DisplayName("Data de Emissão")]
        [DisplayFormat(DataFormatString = "{0:MM-dd-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime EmissionDate { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [DisplayName("Data Limite")]
        [DisplayFormat(DataFormatString = "{0:MM-dd-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime DateLimit { get; set; }

		public Recibo()
        {
            ReciboId = Guid.NewGuid();
        }
    }
}
