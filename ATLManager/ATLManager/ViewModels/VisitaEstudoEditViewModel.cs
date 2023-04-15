using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using ATLManager.Models;

namespace ATLManager.ViewModels
{
    public class VisitaEstudoEditViewModel
    {
        public Guid VisitaEstudoID { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(100)")]
        public string Name { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 5)]
        public string Location { get; set; }

        [Required]
        [StringLength(255)]
        public string Descripton { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Data")]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public string? Date { get; set; }

        [DataType(DataType.Upload)]
        [DisplayName("Local da Visita de Estudo")]
        public IFormFile? Picture { get; set; }

        public VisitaEstudoEditViewModel()
        {
        }

        public VisitaEstudoEditViewModel(VisitaEstudo visitaEstudo)
        {
            VisitaEstudoID = visitaEstudo.VisitaEstudoID;
            Name = visitaEstudo.Name;
            Location = visitaEstudo.Location;
            Descripton = visitaEstudo.Description;
            Date = visitaEstudo.Date.ToShortDateString();
        }
    }
}
