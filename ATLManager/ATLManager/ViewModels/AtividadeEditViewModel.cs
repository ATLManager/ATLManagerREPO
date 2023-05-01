using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using ATLManager.Attributes;

namespace ATLManager.ViewModels
{
    public class AtividadeEditViewModel
    {
		public Guid AtividadeId { get; set; }

		[Required]
		[Display(Name = "Nome")]
        [StringLength(50, ErrorMessage = "Máximo 50 caratéres")]
        public string Name { get; set; }

		[DataType(DataType.Date)]
		[Display(Name = "Data de Emissão")]
		[DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
		public DateTime? StartDate { get; set; }

		[DataType(DataType.Date)]
		[Display(Name = "Data Limite")]
		[DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
		public DateTime? EndDate { get; set; }

		[Required]
        [StringLength(255, ErrorMessage = "Máximo 255 caratéres")]
        [DisplayName("Descrição")]
		public string Description { get; set; }

		[DisplayName("Imagem da atividade")]
        [AllowedExtensions(new string[] { ".jpg", ".jpeg", ".png" },
            ErrorMessage = "A extensão do ficheiro escolhido não é permitida: .jpg, .jpeg, .png")]
		[MaxFileSize(128 * 1024, ErrorMessage = "Tamanho máximo permitido é de 128kB")]
		public IFormFile? Picture { get; set; }
    }
}
