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
            Descripton = descripton;
            Date = date;
        }
    }
}
