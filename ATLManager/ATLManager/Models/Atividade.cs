using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using Xunit.Sdk;

namespace ATLManager.Models
{
    public class Atividade
    {
        [Key]
        public Guid AtividadeId { get; set; }

        [Required]
        [Display(Name = "Nome")]
        [Column(TypeName = "nvarchar(100)")]
        public string Name { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Data de Emissão")]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime StartDate { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Data Limite")]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime EndDate { get; set; }

        [Required]
        [StringLength(255)]
        [DisplayName("Descrição")]
        public string Description { get; set; }

        [DisplayName("Fotografia")]
        public string Picture { get; set; }

        public Atividade()
        {
            AtividadeId = Guid.NewGuid();
        }
    }
}
