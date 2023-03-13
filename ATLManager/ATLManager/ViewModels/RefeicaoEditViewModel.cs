using ATLManager.Areas.Identity.Data;
using ATLManager.Models;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ATLManager.ViewModels
{
    public class RefeicaoEditViewModel
    {
        public Guid RefeicaoId { get; set; }

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

        [Required]
        [DisplayName("Fotografia do menu")]
        public IFormFile Picture { get; set; }

        public RefeicaoEditViewModel(Refeicao refeicao)
        {
            RefeicaoId = refeicao.RefeicaoId;
            Name = refeicao.Name;
            Categoria = refeicao.Categoria;
            Data = refeicao.Data;
            Descricao = refeicao.Descricao;
            Proteina = refeicao.Proteina;
            HidratosCarbono = refeicao.HidratosCarbono;
            VR = refeicao.VR;
            Acucar = refeicao.Acucar;
            Lipidos = refeicao.Lipidos;
            ValorEnergetico = refeicao.ValorEnergetico;
            AGSat = refeicao.AGSat;
            Sal = refeicao.Sal;
        }
    }
}
