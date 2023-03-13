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
        public string Picture { get; set; }

        public Refeicao(string name, string categoria, DateTime data, string descricao,
                        string proteina, string hidratosCarbono, string vR, string acucar, string lipidos,
                        string valorEnergetico, string aGSat, string sal) : this(name, categoria, data, descricao)
        {
            Descricao = descricao;
            Proteina = proteina;
            HidratosCarbono = hidratosCarbono;
            VR = vR;
            Acucar = acucar;
            Lipidos = lipidos;
            ValorEnergetico = valorEnergetico;
            AGSat = aGSat;
            Sal = sal;
        }

        public Refeicao()
        {
            RefeicaoId = Guid.NewGuid();
        }


        public Refeicao(string name, string categoria, DateTime data, string descricao) : this()
        {
            Name = name;
            Data = data;
            Categoria = categoria;
            Descricao = descricao;

        }

        
    }
}
