using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using Xunit.Sdk;

namespace ATLManager.Models
{
    public class VisitaEstudo
    {
        [Key]
        public Guid VisitaEstudoID { get; set; }

        [Required]
        [Display(Name = "Nome")]
        [Column(TypeName = "nvarchar(100)")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Localização")]
        [StringLength(50, MinimumLength = 5)]
        public string Location { get; set; }

        [Required]
        [StringLength(255)]
        [Display(Name = "Descrição")]
        public string Description { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Data")]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime Date { get; set; }

        [DisplayName("Fotografia da Visita de Estudo")]
        public string Picture { get; set; }

        [ForeignKey("Atl")]
        public Guid? AtlId { get; set; }
        public ATL? Atl { get; set; }

        public VisitaEstudo()
        {
            VisitaEstudoID = Guid.NewGuid();
        }

        public VisitaEstudo(string name, string location, string descripton, DateTime date) : this()
        {
            Name = name;
            Location = location;
            Description = descripton;
            Date = date;
        }
    }
}
