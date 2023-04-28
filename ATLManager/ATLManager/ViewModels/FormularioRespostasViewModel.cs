using System.ComponentModel;

namespace ATLManager.ViewModels
{
    public class FormularioRespostasViewModel
    {
        public Guid RespostaId { get; set; }
        public Guid FormularioId { get; set; }

        [DisplayName("Nome do Formulário")]
        public string? Formularioname { get; set; }

        [DisplayName("Nome do Educando")]
        public string EducandoName { get; set; }

        [DisplayName("Autorizado")]
		public bool Authorized { get; set; }

        [DisplayName("Data de resposta")]
		public string ResponseDate { get; set; }
	}
}