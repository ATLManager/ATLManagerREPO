using ATLManager.Areas.Identity.Data;
using ATLManager.Models;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ATLManager.ViewModels
{
    public class RefeicaoCreateViewModel
    {
        [Required]
        [MaxLength(20)]
        public string Name { get; set; }

        [Required]
        [MaxLength(20)]
        public string Categoria { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Data")]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime Data { get; set; }

        [Required]
        [MaxLength(255)]
        public string Descricao { get; set; }

        [RegularExpression(@"^\d+(,\d+)?(.\d+)?$", ErrorMessage = "O valor inserido é inválido")]
        public string Proteina { get; set; }

        [RegularExpression(@"^\d+(,\d+)?(.\d+)?$", ErrorMessage = "O valor inserido é inválido")]
        public string HidratosCarbono { get; set; }

        [RegularExpression(@"^\d+(,\d+)?(.\d+)?$", ErrorMessage = "O valor inserido é inválido")]
        public string VR { get; set; }

        [RegularExpression(@"^\d+(,\d+)?(.\d+)?$", ErrorMessage = "O valor inserido é inválido")]
        public string Acucar { get; set; }

        [RegularExpression(@"^\d+(,\d+)?(.\d+)?$", ErrorMessage = "O valor inserido é inválido")]
        public string Lipidos { get; set; }

        [RegularExpression(@"^\d+(,\d+)?(.\d+)?$", ErrorMessage = "O valor inserido é inválido")]
        public string ValorEnergetico { get; set; }

        [RegularExpression(@"^\d+(,\d+)?(.\d+)?$", ErrorMessage = "O valor inserido é inválido")]
        public string AGSat { get; set; }

        [RegularExpression(@"^\d+(,\d+)?(.\d+)?$", ErrorMessage = "O valor inserido é inválido")]
        public string Sal { get; set; }

        [DisplayName("Fotografia do menu")]
        public IFormFile? Picture { get; set; }
	}
}
