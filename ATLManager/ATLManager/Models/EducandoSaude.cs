using ATLManager.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
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

        [Required]
        [ForeignKey("Educando")]
        public Guid EducandoId { get; set; }
        public Educando Educando { get; set; }

        [Column(TypeName = "nvarchar(3)")]
		public string BloodType { get; set; }

        [Phone]
        [Required]
        [RegularExpression("^9[0-9]{8}$")]
        public string EmergencyContact { get; set; }
        
        public string InsuranceName { get; set; }
        public string InsuranceNumber { get; set; }
        public string Allergies { get; set; }
        public string Diseases { get; set; }
        public string Medication { get; set; }
        public string MedicalHistory { get; set; }

		public EducandoSaude()
		{
			EducandoId = Guid.NewGuid();
		}

        public EducandoSaude(Guid educandoId) : this()
        {
            EducandoId = educandoId;
        }

        public EducandoSaude(Guid educandoId, string bloodType, string emergencyContact, string insuranceName, 
            string insuranceNumber, string allergies, string diseases, string medication, string medicalHistory) : this ()
        {
            EducandoId = educandoId;
            BloodType = bloodType;
            EmergencyContact = emergencyContact;
            InsuranceName = insuranceName;
            InsuranceNumber = insuranceNumber;
            Allergies = allergies;
            Diseases = diseases;
            Medication = medication;
            MedicalHistory = medicalHistory;
        }
    }
}
