using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using ATLManager.Areas.Identity.Data;

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
		[Display(Name = "Data de nascimento")]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime DateOfBirth { get; set; }

		[Required]
		[Column(TypeName = "nvarchar(9)")]
		[Display(Name = "Cartão de Cidadão")]
		public int CC { get; set; }

		public ContaAdministrativa()
		{
			ContaId = Guid.NewGuid();
		}

		public ContaAdministrativa(ATLManagerUser user, DateTime dateOfBirth, int cc) : this ()
		{
			User = user;
			UserId = User.Id;
			DateOfBirth = dateOfBirth;
			CC = cc;
		}
	}
}