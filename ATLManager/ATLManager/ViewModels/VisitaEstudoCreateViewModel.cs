using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using ATLManager.Attributes;
using Xunit.Sdk;

namespace ATLManager.ViewModels
{
    public class VisitaEstudoCreateViewModel
    {

        [Required]
        [StringLength(50, ErrorMessage = "Máximo 50 caratéres")]
        public string Name { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "Máximo 50 caratéres")]
        public string Location { get; set; }

        [Required]
        [StringLength(500)]
        public string Descripton { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Data")]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime Date { get; set; }

        [DisplayName("Local da Visita de Estudo")]
        [AllowedExtensions(new string[] { ".jpg", ".jpeg", ".png" },
            ErrorMessage = "A extensão do ficheiro escolhido não é permitida: .jpg, .jpeg, .png")]
		[MaxFileSize(128 * 1024, ErrorMessage = "Tamanho máximo permitido é de 128kB")]
		public IFormFile? Picture { get; set; }
    }
}
