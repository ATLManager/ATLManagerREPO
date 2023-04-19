using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using Xunit.Sdk;

namespace ATLManager.Models.Historicos
{
    public class AtividadeRecord
    {
        [Key]
        public Guid AtividadeRecordId { get; set; }

        public Guid AtividadeId { get; set; }

        [Display(Name = "Nome")]
        public string Name { get; set; }

        [Display(Name = "Data de Emissão")]
        public DateTime StartDate { get; set; }

        [Display(Name = "Data Limite")]
        public DateTime EndDate { get; set; }

        [DisplayName("Descrição")]
        public string Description { get; set; }

        [DisplayName("Fotografia")]
        public string Picture { get; set; }

        [ForeignKey("Atl")]
        public Guid? AtlId { get; set; }
        public ATL? Atl { get; set; }

        public AtividadeRecord()
        {
            AtividadeRecordId = Guid.NewGuid();
        }
    }
}
