using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ATLManager.Models.DAL
{
    public class ContaUtilizadorDAL
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ContaUtilizador_id { get; set; }

        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Required]
        [Display(Name = "Tipo Conta ID")]
        public int TipoContaId { get; set; }

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

        public ContaUtilizadorDAL( string email, string password, string password1, int tipoContaId, int contaEstadoId, string codigoAtivacao)
        {
            Email = email;
            Password = password;
            TipoContaId = tipoContaId;
            ContaEstadoId = contaEstadoId;
            CodigoAtivacao = codigoAtivacao;
        }
    }
}
