using ATLManager.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
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
		public string CC { get; set; }

		[Required]
		public string Genero { get; set; }

		[ForeignKey("ATL")]
		public Guid AtlId { get; set; }
		public ATL Atl { get; set; }

		[ForeignKey("EncarregadoEducacao")]
		public Guid EncarregadoId { get; set; }
		public EncarregadoEducacao Encarregado { get; set; }

		[Required]
		[DisplayName("Fotografia do educando")]
		public string ProfilePicture { get; set; }

		[Display(Name = "Caminho do ficheiro PDF")]
		public string? DeclaracaoMedica { get; set; }

		[Display(Name = "Caminho do ficheiro PDF")]
		public string? BoletimVacinas { get; set; }

		[Display(Name = "Data de Inscrição")]
		public DateTime DataDeInscricao { get; set; }

		public Educando()
		{
			EducandoId = Guid.NewGuid();
			DataDeInscricao = DateTime.Now;
		}

		public Educando(string name, string apelido, string cc, string genero, Guid atlID, Guid encarregadoID)
		{
			Name = name;
			Apelido = apelido;
			CC = cc;
			Genero = genero;
			AtlId = atlID;
			EncarregadoId = encarregadoID;
			DataDeInscricao = DateTime.Now;
		}
	}
}
