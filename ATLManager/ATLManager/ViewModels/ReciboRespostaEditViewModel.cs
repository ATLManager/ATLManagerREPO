using ATLManager.Attributes;
using ATLManager.Models;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Xunit.Sdk;

namespace ATLManager.ViewModels
{
    public class ReciboRespostaEditViewModel
    {
        public Guid RespostaId { get; set; }
        public Guid ReciboId { get; set; }
		
		[DisplayName("Nome")]
		public string Name { get; set; }

		[DisplayName("Educando")]
		public string Educando { get; set; }

		[DisplayName("Preço")]
		[RegularExpression("[1-9][0-9]*?[,.][0-9]{0,2}", ErrorMessage = "Formato Incorreto, inclua duas casas decimais (Ex. 200,00)")]
		public string Price { get; set; }

		public string NIB { get; set; }

		[DisplayName("Descrição")]
		public string Description { get; set; }

		[DataType(DataType.Date)]
        [DisplayName("Data de resposta")]
		[DisplayFormat(DataFormatString = "{0:MM-dd-yyyy}", ApplyFormatInEditMode = true)]
		public string ResponseDate { get; set; }

		[DataType(DataType.Date)]
		[DisplayName("Data limite de pagamento")]
		[DisplayFormat(DataFormatString = "{0:MM-dd-yyyy}", ApplyFormatInEditMode = true)]
		public string DateLimit { get; set; }

		[DisplayName("Comprovativo")]
        public string ComprovativoPath { get; set; }

		[DisplayName("Autorizado")]
		public bool Authorized { get; set; }

		[DisplayName("Recibo")]
        [AllowedExtensions(new string[] { ".pdf" },
            ErrorMessage = "A extensão do ficheiro escolhido não é permitida: .pdf")]
        public IFormFile? Receipt { get; set; }

		[MaxLength(500)]
		[DisplayName("Notas")]
		public string? Notes { get; set; }

		public ReciboRespostaEditViewModel() { }
	}
}