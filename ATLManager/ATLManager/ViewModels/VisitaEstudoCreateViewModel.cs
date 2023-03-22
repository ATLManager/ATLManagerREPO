using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace ATLManager.ViewModels
{
    public class VisitaEstudoCreateViewModel
    {

        [Required]
        [Column(TypeName = "nvarchar(100)")]
        public string Name { get; set; }

        [Required]
        [StringLength(20, MinimumLength = 5)]
        public string Location { get; set; }

        [Required]
        [StringLength(255)]
        public string Descripton { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Data")]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime Date { get; set; }


        [DisplayName("Local da Visita de Estudo")]
        public IFormFile? Picture { get; set; }
    }
}
