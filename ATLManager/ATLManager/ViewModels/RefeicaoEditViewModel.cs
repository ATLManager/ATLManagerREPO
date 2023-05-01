using ATLManager.Areas.Identity.Data;
using ATLManager.Attributes;
using ATLManager.Models;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Xunit.Sdk;

namespace ATLManager.ViewModels
{
    public class RefeicaoEditViewModel
    {
        public Guid RefeicaoId { get; set; }

        [Required]
        [StringLength(20, ErrorMessage = "Máximo 20 caratéres")]
        public string Name { get; set; }

        [Required]
        [StringLength(20, ErrorMessage = "Máximo 20 caratéres")]
        public string Categoria { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Data")]
        [DisplayFormat(DataFormatString = "{0:MM-dd-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? Data { get; set; }

        [Required]
        [StringLength(20, ErrorMessage = "Máximo 20 caratéres")]
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

        [DataType(DataType.Upload)]
        [DisplayName("Fotografia do menu")]
        [AllowedExtensions(new string[] { ".jpg", ".jpeg", ".png" },
            ErrorMessage = "A extensão do ficheiro escolhido não é permitida: .jpg, .jpeg, .png")]
		[MaxFileSize(128 * 1024, ErrorMessage = "Tamanho máximo permitido é de 128kB")]
		public IFormFile? Picture { get; set; }

        public RefeicaoEditViewModel()
        {
        }

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
