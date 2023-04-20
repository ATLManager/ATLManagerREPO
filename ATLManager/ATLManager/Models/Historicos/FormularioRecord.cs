using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ATLManager.Models.Historicos
{
    public class FormularioRecord
    {
        [Key]
        public Guid FormularioRecordId { get; set; }
        public Guid FormularioId { get; set; }

        [DisplayName("Nome")]
        public string Name { get; set; }

        [DisplayName("Descrição")]
        public string Description { get; set; }

        [DisplayName("Visita de Estudo")]
        public string? VisitaEstudo { get; set; }

        [DisplayName("Atividade")]
        public string? Atividade { get; set; }

        [DisplayName("Data de Emissão")]
        public DateTime StartDate { get; set; }

        [DisplayName("Data Limite")]
        public DateTime DateLimit { get; set; }

        [ForeignKey("Atl")]
        public Guid? AtlId { get; set; }
        public ATL? Atl { get; set; }

        public FormularioRecord()
        {
            FormularioRecordId = Guid.NewGuid();
        }
    }
}
