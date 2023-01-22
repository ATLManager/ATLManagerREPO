namespace ATLManager.Models.BLL
{
    public class Administrador
    {
        public string nome { get; set; }
        public string apelido { get; set; }
        public DateTime data_nascimento { get; set; }
        public int cc { get; set; }
        public string certidao_permanente { get; set; }

        public Administrador(string nome, string apelido, DateTime data_nascimento, int cc, string certidao_permanente)
        {
            this.nome = nome;
            this.apelido = apelido;
            this.data_nascimento = data_nascimento;
            this.cc = cc;
            this.certidao_permanente = certidao_permanente;
        }
    }
}
