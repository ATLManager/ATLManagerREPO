using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ATLManager.Models
{
    public class Conta
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid ContaID { get; set; }

        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }
        
        [Required]
        [Display(Name = "Username")]
        public string Username { get; set; }

        [Required]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Required]
        [Display(Name = "Tipo Conta")]
        public ContaRoles ContaRole { get; set; }

        [Required]
        [Display(Name = "Conta Estado ID")]
        public int ContaEstadoId { get; set; }

        [Required]
        [Display(Name = "Codigo Ativacao")]
        public string CodigoAtivacao { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Data Criacao")]
        public DateTime DataCriacao { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Data Ultima Alteracao")]
        public DateTime DataUltimaAlteracao { get; set; }

        public Conta()
        {
            this.ContaID = Guid.NewGuid();
            this.DataCriacao = DateTime.UtcNow;
        }

        public Conta(string Email, string Username, string Password, ContaRoles ContaRole, int ContaEstadoId, string CodigoAtivacao) : this ()
        {
            this.Email = Email;
            this.Username = Username;
            this.Password = Password;
            this.ContaRole = ContaRole;
            this.ContaEstadoId = ContaEstadoId;
            this.CodigoAtivacao = CodigoAtivacao;
        }
    }
}
