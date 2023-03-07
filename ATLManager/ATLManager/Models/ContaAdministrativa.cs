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

		[Required]
		public ATLManagerUser User { get; set; }

		[Required]
		[ForeignKey("User")]
		public Guid UserId { get; set; }

		[Required]
		[DataType(DataType.Date)]
		[Display(Name = "Data de nascimento")]
		public DateOnly DateOfBirth { get; set; }

		[Required]
		[Column(TypeName = "nvarchar(9)")]
		[Display(Name = "Cartão de Cidadão")]
		public int CC { get; set; }

		public ContaAdministrativa()
		{
			ContaId = Guid.NewGuid();
		}

		public ContaAdministrativa(ATLManagerUser user, DateOnly dateOfBirth, int cc)
		{
			User = user;
			UserId = user.UserId;
			DateOfBirth = dateOfBirth;
			CC = cc;
		}
	}
}