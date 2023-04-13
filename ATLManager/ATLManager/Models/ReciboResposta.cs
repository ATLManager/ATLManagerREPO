using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ATLManager.Models
{
    public class ReciboResposta
    {
        [Key]
        public Guid ReciboRespostaId { get; set; }

        [Required]
        [ForeignKey("Recibo")]
        public Guid ReciboId { get; set; }
        public Recibo Recibo { get; set; }

        [Required]
        [ForeignKey("Educando")]
        public Guid EducandoId { get; set; }
        public Educando Educando { get; set; }
        
        public bool Authorized { get; set; } = false;

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:MM-dd-yyyy}", ApplyFormatInEditMode = true)]
		[DisplayName("Data limite de pagamento")]
		public DateTime DateLimit { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:MM-dd-yyyy}", ApplyFormatInEditMode = true)]
        [DisplayName("Data de início do formulário")]
        public DateTime? ResponseDate { get; set; }

        [DisplayName("Comprovativo")]
        public string? ComprovativoPath { get; set; }

        public ReciboResposta()
        {
            ReciboRespostaId = Guid.NewGuid();
        }

        public ReciboResposta(Guid reciboId, Guid educandoId) : this ()
        {
            ReciboId = reciboId;
            EducandoId = educandoId;
        }
    }
}
