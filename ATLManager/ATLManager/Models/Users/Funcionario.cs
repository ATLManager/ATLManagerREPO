using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ATLManager.Models
{
    public class Funcionario
    {
        [Key]
        public Guid FuncionarioID { get; set; }

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

        public Funcionario(string firstName, string lastName, DateTime dateOfBirth, int cc)
        {
            FirstName = firstName;
            LastName = lastName;
            DateOfBirth = dateOfBirth;
            CC = cc;
        }
    }
}
