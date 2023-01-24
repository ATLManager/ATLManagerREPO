namespace ATLManager.Models.BLL
{
    public class Coordenador
    {
        public string Nome { get; set; }
        public string Apelido { get; set; }
        public DateTime DataNascimento { get; set; }
        public int Cc { get; set; }

        public Coordenador(string nome, string apelido, DateTime dataNascimento, int cc)
        {
            Nome = nome;
            Apelido = apelido;
            DataNascimento = dataNascimento;
            Cc = cc;
        }
    }
}
