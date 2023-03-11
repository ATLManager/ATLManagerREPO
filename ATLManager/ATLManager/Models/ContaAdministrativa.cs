using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using ATLManager.Areas.Identity.Data;
using System.ComponentModel;

namespace ATLManager.Models
{
    public class ContaAdministrativa
    {
		[Key]
		public Guid ContaId { get; set; }

        [ForeignKey("User")]
        public string UserId { get; set; }

		public ATLManagerUser User { get; set; }

		[Required]
		[DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
		[DisplayName("Data de nascimento")]
        public DateTime DateOfBirth { get; set; }

		[Required]
        [DisplayName("Cartão de cidadão")]
        [StringLength(9, MinimumLength = 9, ErrorMessage = "Este campo deve conter 9 dígitos")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Este campo deve conter apenas dígitos")]
        public string CC { get; set; }

		[ForeignKey("Atl")]
		public Guid? AtlId { get; set; }

		public ATL? Atl { get; set; }

		public ContaAdministrativa()
		{
			ContaId = Guid.NewGuid();
		}

		public ContaAdministrativa(ATLManagerUser user, DateTime dateOfBirth, string cc) : this ()
		{
			User = user;
			UserId = User.Id;
			DateOfBirth = dateOfBirth;
			CC = cc;
		}

        public ContaAdministrativa(ATLManagerUser user, ATL atl, DateTime dateOfBirth, string cc) : this()
        {
            User = user;
            UserId = User.Id;
			Atl = atl;
			AtlId = atl.AtlId;
            DateOfBirth = dateOfBirth;
            CC = cc;
        }
    }
}