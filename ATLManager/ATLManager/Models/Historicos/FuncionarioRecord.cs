using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using ATLManager.Areas.Identity.Data;
using System.ComponentModel;

namespace ATLManager.Models.Historicos
{
    public class FuncionarioRecord
    {
		[Key]
		public Guid FuncionarioRecordId { get; set; }
		public Guid ContaId { get; set; }
        
        [DisplayName("Nome")]
        public string FirstName { get; set; }
        
        [DisplayName("Apelido")]
        public string LastName { get; set; }

        [DisplayName("Email")]
        public string Email { get; set; }

        [DisplayName("Data de nascimento")]
        public DateTime DateOfBirth { get; set; }

        [DisplayName("Cartão de cidadão")]
        public string CC { get; set; }

        [Required]
        [DisplayName("Fotografia")]
        public string ProfilePicture { get; set; }
        
		[ForeignKey("Atl")]
		public Guid? AtlId { get; set; }
		public ATL? Atl { get; set; }

        public FuncionarioRecord()
		{
            FuncionarioRecordId = Guid.NewGuid();
		}
    }
}