using ATLManager.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ATLManager.Models
{
    public class Educando
    {
        [Key]
        public Guid EducandoId { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(100)")]
		public string Name { get; set; }

		[Required]
        [Column(TypeName = "nvarchar(100)")]
        public string Apelido { get; set; }

        [Required]
        [StringLength(9, MinimumLength = 9, ErrorMessage = "Este campo deve conter 9 dígitos")]
        public int NIF { get; set; }

        [Required]
        public string Genero { get; set; }

        
        [ForeignKey("ATL")]
        public Guid AtlId { get; set; }

        public ATL Atl{ get; set; }


        [ForeignKey("Encarregado de Educação")]
        public Guid EncarregadoId { get; set; }

        public EncarregadoEducacao Encarregado { get; set; }

        /*
        [Display(Name = "Caminho do ficheiro PDF")]
        public string DeclaracaoMedicaPath { get; set; }

        [NotMapped]
        [Display(Name = "Carregamento Ficheiro PDF")]
        public IFormFile DeclaracaoMedicaPDF { get; set; }


        [Display(Name = "Caminho do ficheiro PDF")]
        public string BoletimVacinasPath { get; set; }

        [NotMapped]
        [Display(Name = "Carregamento  Ficheiro PDF")]
        public IFormFile BoletimVacinasPDF { get; set; }

        [Display(Name = "Imagem")]
        public byte[] ImagemDados { get; set; }

        [Display(Name = "Nome da Imagem")]
        public string ImagemNome { get; set; }

        [NotMapped]
        [DisplayName("Carregamento de Imagem")]
        public IFormFile ImagemArquivo { get; set; }

        */

        public Educando()
		{
			EducandoId = Guid.NewGuid();
		}

        public Educando(string name, string apelido, int nIF, string genero, Guid atlID, Guid encarregadoID)
        {
            Name = name;
            Apelido = apelido;
            NIF = nIF;
            Genero = genero;
            AtlId = atlID;
            EncarregadoId = encarregadoID;
        }

        /*
        public Educando(string name, string apelido, int nIF, string genero,
                string aTLPertencente, string declaracaoMedicaPath,
                string boletimVacinasPath, byte[] imagemDados) : this(name, apelido, nIF, genero, aTLPertencente)
        {
            DeclaracaoMedicaPath = declaracaoMedicaPath;
            BoletimVacinasPath = boletimVacinasPath;
            ImagemDados = imagemDados;
        } */

    }
}
