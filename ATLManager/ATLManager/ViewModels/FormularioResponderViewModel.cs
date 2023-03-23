using ATLManager.Areas.Identity.Data;
using ATLManager.Models;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ATLManager.ViewModels
{
    public class FormularioResponderViewModel
    {
		public Guid FormularioRespostaId { get; set; }

		[DisplayName("Nome do formulário")]
		public string Name { get; set; }

		[DisplayName("Descrição")]
		public string Description { get; set; }

		[DisplayName("Data Limite")]
		public string DateLimit { get; set; }

		[DisplayName("Autorizo")]
		public bool Authorized { get; set; }
	}
}
