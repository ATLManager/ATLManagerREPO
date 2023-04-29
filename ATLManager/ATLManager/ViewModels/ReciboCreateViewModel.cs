using ATLManager.Areas.Identity.Data;
using ATLManager.Models;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ATLManager.ViewModels
{
    public class ReciboCreateViewModel
    {
        [Required]
        [MaxLength(30)]
        [DisplayName("Nome")]
        public string Name { get; set; }

		[Required]
		[DisplayName("Preço")]
		[RegularExpression("[1-9][0-9]*?[,.][0-9]{0,2}", ErrorMessage = "Formato Incorreto: O preço deve conter um valor decimal")]
		public string Price { get; set; }

		[Required]
        [StringLength(21, MinimumLength = 21, ErrorMessage = "Este campo deve conter 21 dígitos")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Este campo deve conter apenas dígitos")]
        public string NIB { get; set; }

        [Required]
        [MaxLength(500)]
        public string Description { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Data")]
        [DisplayFormat(DataFormatString = "{0:MM-dd-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime DateLimit { get; set; }

        public Guid? Educando { get; set; }
    }
}
