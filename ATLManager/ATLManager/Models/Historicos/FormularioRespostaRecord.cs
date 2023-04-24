using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ATLManager.Models.Historicos
{
    public class FormularioRespostaRecord
    {
        [Key]
        public Guid FormularioRespostaRecordId { get; set; }
        public Guid FormularioRespostaId { get; set; }

        [ForeignKey("FormularioRecord")]
        public Guid FormularioRecordId { get; set; }
        public FormularioRecord FormularioRecord { get; set; }

        [DisplayName("Educando")]
        public string Educando { get; set; }

        [DisplayName("Autorizado")]
        public bool Authorized { get; set; }

        [DisplayName("Data limite de resposta")]
        public DateTime? DateLimit { get; set; }

        [DisplayName("Data da resposta")]
        public DateTime? ResponseDate { get; set; }

        public FormularioRespostaRecord()
        {
            FormularioRespostaId = Guid.NewGuid();
        }
    }
}
