using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Drawing;
using System.Net;
using System.Runtime.CompilerServices;

namespace ATLManager.Models.Historicos
{
    public class RefeicaoRecord
    {
        [Key]
        public Guid RefeicaoRecordId { get; set; }
        public Guid RefeicaoId { get; set; }

        [Display(Name = "Nome")]
        public string Name { get; set; }

        [Display(Name = "Categoria")]
        public string Categoria { get; set; }

        [Display(Name = "Data")]
        public DateTime Data { get; set; }

        [Display(Name = "Descrição")]
        public string Descricao { get; set; }

        [Display(Name = "Proteína")]
        public string Proteina { get; set; }

        [Display(Name = "Hidratos de Carbono")]
        public string HidratosCarbono { get; set; }

        [Display(Name = "VR")]
        public string VR { get; set; }

        [Display(Name = "Açúcar")]
        public string Acucar { get; set; }

        [Display(Name = "Lípidos")]
        public string Lipidos { get; set; }

        [Display(Name = "Valor Energético")]
        public string ValorEnergetico { get; set; }

        [Display(Name = "AG Saturados")]
        public string AGSat { get; set; }

        [Display(Name = "Sal")]
        public string Sal { get; set; }

        [Required]
        [DisplayName("Fotografia do menu")]
        public string Picture { get; set; }

        [ForeignKey("Atl")]
        public Guid? AtlId { get; set; }
        public ATL? Atl { get; set; }

        public RefeicaoRecord()
        {
            RefeicaoRecordId = Guid.NewGuid();
        }
    }
}
