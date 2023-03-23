using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ATLManager.Models
{
    public class Formulario
    {
        [Key]
        public Guid FormularioId { get; set; }

		[Required]
        [MaxLength(20)]
        public string Name { get; set; }

        [Required]
        [MaxLength(1000)]
        public string Description { get; set; }

        [Required]
        [ForeignKey("VisitaEstudo")]
        public Guid? VisitaEstudoId { get; set; }
        public VisitaEstudo? VisitaEstudo { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:MM-dd-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime StartDate { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:MM-dd-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime DateLimit { get; set; }

        public Formulario()
        {
            FormularioId = Guid.NewGuid();
        }
    }
}
