using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using Xunit.Sdk;

namespace ATLManager.Models.Historicos
{
    public class VisitaEstudoRecord
    {
        [Key]
        public Guid VisitaEstudoRecordID { get; set; }
        public Guid VisitaEstudoID { get; set; }

        [Display(Name = "Nome")]
        public string Name { get; set; }

        [Display(Name = "Localização")]
        public string Location { get; set; }

        [Display(Name = "Descrição")]
        public string Description { get; set; }

        [Display(Name = "Data")]
        public DateTime Date { get; set; }

        [DisplayName("Fotografia da Visita de Estudo")]
        public string Picture { get; set; }

        [ForeignKey("Atl")]
        public Guid? AtlId { get; set; }
        public ATL? Atl { get; set; }

        public VisitaEstudoRecord()
        {
            VisitaEstudoRecordID = Guid.NewGuid();
        }
    }
}
