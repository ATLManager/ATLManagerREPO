using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Drawing;
using System.Net;
using System.Runtime.CompilerServices;

namespace ATLManager.Models
{
    public class Refeicao
    {
        [Key]
        public Guid RefeicaoId { get; set; }

        [Required]
        [Display(Name = "Nome")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Categoria")]
        public string Categoria { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Data")]
        [DisplayFormat(DataFormatString = "{0:MM-dd-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime Data { get; set; }

        [Required]
        [Display(Name = "Descrição")]
        public string Descricao { get; set; }

        [Display(Name = "Proteína")]
        [RegularExpression(@"^\d+(,\d+)?(.\d+)?$", ErrorMessage = "O valor inserido é inválido")]
        public string Proteina { get; set; }

        [Display(Name = "Hidratos de Carbono")]
        [RegularExpression(@"^\d+(,\d+)?(.\d+)?$", ErrorMessage = "O valor inserido é inválido")]
        public string HidratosCarbono { get; set; }

        [Display(Name = "VR")]
        [RegularExpression(@"^\d+(,\d+)?(.\d+)?$", ErrorMessage = "O valor inserido é inválido")]
        public string VR { get; set; }

        [Display(Name = "Açúcar")]
        [RegularExpression(@"^\d+(,\d+)?(.\d+)?$", ErrorMessage = "O valor inserido é inválido")]
        public string Acucar { get; set; }

        [Display(Name = "Lípidos")]
        [RegularExpression(@"^\d+(,\d+)?(.\d+)?$", ErrorMessage = "O valor inserido é inválido")]
        public string Lipidos { get; set; }

        [Display(Name = "Valor Energético")]
        [RegularExpression(@"^\d+(,\d+)?(.\d+)?$", ErrorMessage = "O valor inserido é inválido")]
        public string ValorEnergetico { get; set; }

        [Display(Name = "AG Saturados")]
        [RegularExpression(@"^\d+(,\d+)?(.\d+)?$", ErrorMessage = "O valor inserido é inválido")]
        public string AGSat { get; set; }

        [Display(Name = "Sal")]
        [RegularExpression(@"^\d+(,\d+)?(.\d+)?$", ErrorMessage = "O valor inserido é inválido")]
        public string Sal { get; set; }

        [Required]
        [DisplayName("Fotografia do menu")]
        public string Picture { get; set; }

        [ForeignKey("Atl")]
        public Guid? AtlId { get; set; }
        public ATL? Atl { get; set; }

        public Refeicao()
        {
            RefeicaoId = Guid.NewGuid();
        }
    }
}
