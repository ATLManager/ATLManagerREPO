using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using ATLManager.Models;
using ATLManager.Attributes;
using Xunit.Sdk;

namespace ATLManager.ViewModels
{
    public class VisitaEstudoEditViewModel
    {
        public Guid VisitaEstudoID { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(100)")]
        [StringLength(30, MinimumLength = 5)]
        public string Name { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 5)]
        public string Location { get; set; }

        [Required]
        [StringLength(255)]
        public string Descripton { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Data")]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public string? Date { get; set; }

        [DataType(DataType.Upload)]
        [DisplayName("Local da Visita de Estudo")]
        [AllowedExtensions(new string[] { ".jpg", ".jpeg", ".png" },
            ErrorMessage = "A extensão do ficheiro escolhido não é permitida: .jpg, .jpeg, .png")]
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
