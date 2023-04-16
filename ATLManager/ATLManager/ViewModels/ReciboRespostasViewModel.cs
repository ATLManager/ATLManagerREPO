using System.ComponentModel;

namespace ATLManager.ViewModels
{
    public class ReciboRespostasViewModel
    {
        public Guid RespostaId { get; set; }
        public Guid ReciboId { get; set; }

        [DisplayName("Nome do Educando")]
        public string EducandoName { get; set; }

        [DisplayName("Autorizado")]
		public bool Authorized { get; set; }

        [DisplayName("Data de resposta")]
		public string ResponseDate { get; set; }

        public string ComprovativoPath { get; set; }
	}
}