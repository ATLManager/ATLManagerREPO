using ATLManager.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ATLManager.Models.Historicos
{
    public class EducandoRecord
    {
        [Key]
        public Guid EducandoRecordId { get; set; }

        public Guid EducandoId { get; set; }

        [DisplayName("Nome")]
		public string Name { get; set; }

        [DisplayName("Apelido")]
        public string Apelido { get; set; }

        [DisplayName("CC")]
        public string CC { get; set; }

        [DisplayName("Género")]
        public string Genero { get; set; }

        [ForeignKey("Encarregado de Educacao")]
        public string Encarregado { get; set; }

		[DisplayName("Fotografia do educando")]
		public string ProfilePicture { get; set; }
        
        [ForeignKey("ATL")]
        public Guid AtlId { get; set; }
        public ATL Atl{ get; set; }

		public EducandoRecord()
		{
			EducandoRecordId = Guid.NewGuid();
		}
    }
}
