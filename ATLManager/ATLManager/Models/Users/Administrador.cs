using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ATLManager.Models
{
    public class Administrador
    {
        [Key]
        public Guid AdministradorID { get; set; }

        [Required]
        [MaxLength(20)]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(20)]
        public string LastName { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime DateOfBirth { get; set; }
        
        [Required]
        [Column(TypeName = "nvarchar(9)")]
        public int CC { get; set; }

        [Required]
        [StringLength(14, MinimumLength = 14, ErrorMessage = "Este campo deve conter 14 dígitos")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Este campo deve conter apenas dígitos")]
        public string CertidaoPermanente { get; set; }

        public Administrador() { 
            AdministradorID = Guid.NewGuid();
        }

        public Administrador(string firstName, string lastName, DateTime dateOfBirth, int cc, string certidaoPermanente) : this ()
        {
            FirstName = firstName;
            LastName = lastName;
            DateOfBirth = dateOfBirth;
            CC = cc;
            CertidaoPermanente = certidaoPermanente;
        }
    }
}
