namespace ATLManager.Models.BLL
{
    public class Conta
    {
        public string email { get; set; }
        public string username { get; set; }
        public string password { get; set; }
        public byte[] imagem { get; set; }
        public string tipo_conta { get; set; }
        public string conta_estado { get; set; }
        public string codigo_ativacao { get; set; }

        public Conta(string email, string username, string password, byte[] imagem, string tipo_conta, string conta_estado, string codigo_ativacao)
        {
            this.email = email;
            this.username = username;
            this.password = password;
            this.imagem = imagem;
            this.tipo_conta = tipo_conta;
            this.conta_estado = conta_estado;
            this.codigo_ativacao = codigo_ativacao;
        }
    }
}
