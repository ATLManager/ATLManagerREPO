using ATLManager.Areas.Identity.Data;
using ATLManager.Models;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ATLManager.ViewModels
{
    public class EducandoEditViewModel
    {
		public Guid EducandoId { get; set; }

		[Required]
		[Column(TypeName = "nvarchar(100)")]
		public string Name { get; set; }

		[Required]
		[Column(TypeName = "nvarchar(100)")]
		public string Apelido { get; set; }

		[Required]
		[StringLength(9, MinimumLength = 9, ErrorMessage = "Este campo deve conter 9 dígitos")]
		public string CC { get; set; }

		[Required]
		public string Genero { get; set; }
		public Guid AtlId { get; set; }
		public Guid EncarregadoId { get; set; }

		[Required]
		[DisplayName("Imagem da criança")]
		public IFormFile ProfilePicture { get; set; }

        public EducandoEditViewModel(Educando educando)
        {
            EducandoId = educando.EducandoId;
            Name = educando.Name;
            Apelido = educando.Apelido;
            CC = educando.CC;
            Genero = educando.Genero;
            AtlId = educando.AtlId;
            EncarregadoId = educando.EncarregadoId;
        }
    }
}
