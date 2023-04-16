using ATLManager.Models;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ATLManager.ViewModels
{
    public class ReciboRespostaEditViewModel
    {
        public Guid RespostaId { get; set; }
        public Guid ReciboId { get; set; }

        [ForeignKey("Educando")]
        public Guid EducandoId { get; set; }
        public Educando Educando { get; set; }

		[DisplayName("Nome")]
		public string Name { get; set; }
		[DisplayName("Preço")]
		public decimal Price { get; set; }
		public string NIB { get; set; }
		[DisplayName("Descrição")]
		public string Description { get; set; }
        [DisplayName("Data de resposta")]
		public string ResponseDate { get; set; }
		[DisplayName("Data limite de pagamento")]
		public DateTime DateLimit { get; set; }
		[DisplayName("Comprovativo")]
        public string ComprovativoPath { get; set; }

		[DisplayName("Autorizado")]
		public bool Authorized { get; set; }

		[DisplayName("Recibo")]
		public IFormFile? Receipt { get; set; }

		[MaxLength(500)]
		[DisplayName("Notas")]
		public string? Notes { get; set; }

		public ReciboRespostaEditViewModel() { }

		public ReciboRespostaEditViewModel(ReciboResposta resposta)
		{
			RespostaId = resposta.ReciboRespostaId;
			ReciboId = resposta.ReciboId;
			EducandoId = resposta.EducandoId;
			Name = resposta.Name;
			Price = resposta.Price;
			NIB = resposta.NIB;
			Description = resposta.Description;
			ResponseDate = resposta.ResponseDate.ToString();
			DateLimit = resposta.DateLimit;
			ComprovativoPath = resposta.ComprovativoPath;
			Authorized = resposta.Authorized;
			Notes = resposta.Notes;
		}
	}
}