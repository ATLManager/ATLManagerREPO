using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using ATLManager.Attributes;

namespace ATLManager.ViewModels
{
    public class AtividadeCreateViewModel
    {
		[Required]
		[Display(Name = "Nome")]
		[Column(TypeName = "nvarchar(100)")]
		public string Name { get; set; }

		[Required]
		[DataType(DataType.Date)]
		[Display(Name = "Data de Emissão")]
		[DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
		public DateTime StartDate { get; set; }

		[Required]
		[DataType(DataType.Date)]
		[Display(Name = "Data Limite")]
		[DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
		public DateTime EndDate { get; set; }

		[Required]
		[StringLength(255)]
		[DisplayName("Descrição")]
		public string Description { get; set; }

		[DisplayName("Imagem da atividade")]
        [AllowedExtensions(new string[] { ".jpg", ".jpeg", ".png" },
            ErrorMessage = "A extensão do ficheiro escolhido não é permitida: .jpg, .jpeg, .png")]
        public IFormFile? Picture { get; set; }
    }
}
