using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ATLManager.Models
{
    public class FormularioResposta
    {
        [Key]
        public Guid FormularioRespostaId { get; set; }

        [Required]
        [ForeignKey("Formulario")]
        public Guid FormularioId { get; set; }
        public Formulario Formulario { get; set; }

        [Required]
        [ForeignKey("Educando")]
        public Guid EducandoId { get; set; }
        public Educando Educando { get; set; }
        
        public bool Authorized { get; set; } = false;

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:MM-dd-yyyy}", ApplyFormatInEditMode = true)]
		[DisplayName("Data limite de resposta")]
		public DateTime? DateLimit { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:MM-dd-yyyy}", ApplyFormatInEditMode = true)]
        [DisplayName("Data de início do formulário")]
        public DateTime? ResponseDate { get; set; }

        public FormularioResposta ()
        {
            FormularioRespostaId = Guid.NewGuid();
        }

        public FormularioResposta(Guid formularioId, Guid educandoId) : this ()
        {
            FormularioId = formularioId;
            EducandoId = educandoId;
        }
    }
}
