using ATLManager.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ATLManager.Models
{
    public class EducandoSaude
    {
        [Key]
        public Guid EducandoSaudeId { get; set; }

        [ForeignKey("Educando")]
        public Guid EducandoId { get; set; }
        [BindNever]
        public Educando? Educando { get; set; }

        [StringLength(3)]
        [DisplayName("Tipo sanguíneo")]
		public string? BloodType { get; set; }

        [Phone]
        [DisplayName("Contacto de emergência")]
        [StringLength(9, MinimumLength = 9)]
		[RegularExpression("^[1-9][0-9]{8}$", ErrorMessage = "Formato Incorreto - Introduza um número de 9 dígitos")]
		public string? EmergencyContact { get; set; }
        
        [StringLength(25)]
        [DisplayName("Nome do seguro")]
        public string? InsuranceName { get; set; }
        
        [StringLength(25)]
        [DisplayName("Número do seguro")]
        public string? InsuranceNumber { get; set; }

        [StringLength(255)]
        [DisplayName("Doenças")]
        public string? Allergies { get; set; }

        [StringLength(255)]
        [DisplayName("Doenças")]
        public string? Diseases { get; set; }

        [StringLength(255)]
        [DisplayName("Medicação")]
        public string? Medication { get; set; }

        [StringLength(500)]
        [DisplayName("Histórico Médico")]
        public string? MedicalHistory { get; set; }

		public EducandoSaude()
		{
			EducandoId = Guid.NewGuid();
		}

        public EducandoSaude(Guid educandoId) : this()
        {
            EducandoId = educandoId;
        }
    }
}
