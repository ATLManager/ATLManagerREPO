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

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:MM-dd-yyyy}", ApplyFormatInEditMode = true)]
		[DisplayName("Data limite de pagamento")]
		public DateTime DateLimit { get; set; }
        public bool Authorized { get; set; } = false;

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:MM-dd-yyyy}", ApplyFormatInEditMode = true)]
        [DisplayName("Data de Resposta")]
        public DateTime? ResponseDate { get; set; }

        [DisplayName("Comprovativo")]
        public string? ComprovativoPath { get; set; }

        [DisplayName("Recibo")]
        public string? ReceiptPath { get; set; }

        [MaxLength(500)]
        [DisplayName("Notas")]
        public string? Notes { get; set; }

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
