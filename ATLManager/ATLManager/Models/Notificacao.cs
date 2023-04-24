using ATLManager.Areas.Identity.Data;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ATLManager.Models
{
	public class Notificacao
	{
		[Key]
		public Guid NotificacaoId { get; set; }

		[Required]
		public string UserId { get; set; }

		[ForeignKey("UserId")]
        public virtual ATLManagerUser User { get; set; }

        [Required]
        public string Titulo { get; set; }
		
        [Required]
		public string Mensagem { get; set; }

		public bool Lida { get; set; } = false;

		public DateTime DataNotificacao { get; set; } = DateTime.Now;

		public Notificacao() { }

        public Notificacao(string userId, string titulo, string mensagem)
		{
			UserId = userId;
            Titulo = titulo;
            Mensagem = mensagem;
		}
	}
}
