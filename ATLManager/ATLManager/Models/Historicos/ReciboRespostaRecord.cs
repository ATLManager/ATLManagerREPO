using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ATLManager.Models.Historicos
{
    public class ReciboRespostaRecord
    {
        [Key]
        public Guid ReciboRespostaRecordId { get; set; }

        [ForeignKey("ReciboRecord")]
        public Guid ReciboRecordId { get; set; }
        public ReciboRecord ReciboRecord { get; set; }

        [DisplayName("Educando")]
        public string Educando { get; set; }

        [DisplayName("Nome")]
        public string Name { get; set; }

        [DisplayName("Preço")]
        public string Price { get; set; }

        [DisplayName("NIB")]
        public string NIB { get; set; }

        [DisplayName("Descrição")]
        public string Description { get; set; }

        [DisplayName("Data limite de pagamento")]
        public DateTime DateLimit { get; set; }

        [DisplayName("Autorizado")]
        public bool Authorized { get; set; }

        [DisplayName("Data de Resposta")]
        public DateTime? ResponseDate { get; set; }

        [DisplayName("Comprovativo")]
        public string? ComprovativoPath { get; set; }

        [DisplayName("Recibo")]
        public string? ReceiptPath { get; set; }

        [MaxLength(500)]
        [DisplayName("Notas")]
        public string? Notes { get; set; }

        public ReciboRespostaRecord()
        {
            ReciboRespostaRecordId = Guid.NewGuid();
        }
    }
}
