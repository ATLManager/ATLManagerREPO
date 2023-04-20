using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ATLManager.Models.Historicos
{
    public class ReciboRecord
    {
        [Key]
        public Guid ReciboRecordId { get; set; }

        [DisplayName("Nome")]
        public string Name { get; set; }

        [DisplayName("Preço")]
        public string Price { get; set; }

        [DisplayName("NIB")]
        public string NIB { get; set; }

        [DisplayName("Descrição")]
        public string Description { get; set; }

        [DisplayName("Data de Emissão")]
        public DateTime EmissionDate { get; set; }

        [DisplayName("Data Limite")]
        public DateTime DateLimit { get; set; }

        [ForeignKey("Atl")]
        public Guid? AtlId { get; set; }
        public ATL? Atl { get; set; }

        public ReciboRecord()
        {
            ReciboRecordId = Guid.NewGuid();
        }
    }
}
