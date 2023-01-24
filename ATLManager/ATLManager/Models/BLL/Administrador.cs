namespace ATLManager.Models.BLL
{
    public class Administrador
    {
        public string Nome { get; set; }
        public string Apelido { get; set; }
        public DateTime DataNascimento { get; set; }
        public int Cc { get; set; }
        public string CertidaoPermanente { get; set; }


        public Administrador(string nome, string apelido, DateTime dataNascimento, int cc, string certidaoPermanente)
        {
            Nome = nome;
            Apelido = apelido;
            DataNascimento = dataNascimento;
            Cc = cc;
            CertidaoPermanente = certidaoPermanente;
        }
    }
}
