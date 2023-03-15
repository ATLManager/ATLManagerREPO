using ATLManager.Areas.Identity.Data;
using ATLManager.Models;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ATLManager.ViewModels
{
    public class LowerAccountEditViewModel
    {
        public Guid ContaId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public Guid AtlId { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        [DisplayName("Data de nascimento")]
        public string? DateOfBirth { get; set; }

        [Required]
        [StringLength(9, MinimumLength = 9, ErrorMessage = "Este campo deve conter 9 dígitos")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Este campo deve conter apenas dígitos")]
        public string CC { get; set; }
        public string Email { get; set; }

        [DataType(DataType.Upload)]
        [DisplayName("Imagem de perfil")]
        public IFormFile? ProfilePicture { get; set; }
    }
}
