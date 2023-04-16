using ATLManager.Areas.Identity.Data;
using ATLManager.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ATLManager.ViewModels
{
    public class ReciboResponderViewModel
    {
		public Guid ReciboRespostaId { get; set; }

		[DisplayName("Nome do formulário")]
		public string Name { get; set; }
		
		[DisplayName("NIB")]
		public string NIB { get; set; }

		[DisplayName("Preço")]
		public decimal Price { get; set; }

		[DisplayName("Descrição")]
		public string Description { get; set; }

		[DisplayName("Data Limite")]
		public string DateLimit { get; set; }

        [DisplayName("Comprovativo")]
        public IFormFile Comprovativo { get; set; }
	}
}
