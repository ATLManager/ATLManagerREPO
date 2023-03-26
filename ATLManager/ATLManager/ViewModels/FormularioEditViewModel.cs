using ATLManager.Areas.Identity.Data;
using ATLManager.Models;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ATLManager.ViewModels
{
	public class FormularioEditViewModel
	{
		public Guid FormularioId { get; set; }

		[MaxLength(20)]
		public string Name { get; set; }

		[MaxLength(1000)]
		public string Description { get; set; }

		[ForeignKey("VisitaEstudo")]
		public Guid? VisitaEstudoId { get; set; }
		public VisitaEstudo? VisitaEstudo { get; set; }

		[DataType(DataType.Date)]
		[DisplayFormat(DataFormatString = "{0:MM-dd-yyyy}", ApplyFormatInEditMode = true)]
		public string? StartDate { get; set; }

		[DataType(DataType.Date)]
		[DisplayFormat(DataFormatString = "{0:MM-dd-yyyy}", ApplyFormatInEditMode = true)]
		public string? DateLimit { get; set; }
	}
}
