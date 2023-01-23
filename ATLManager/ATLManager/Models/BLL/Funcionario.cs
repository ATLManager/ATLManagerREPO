namespace ATLManager.Models.BLL
{
    public class Funcionario
    {
        public string Nome { get; set; }
        public string Apelido { get; set; }
        public DateTime DataNascimento { get; set; }
        public int Cc { get; set; }

        public Funcionario(string nome, string apelido, DateTime dataNascimento, int cc)
        {
            Nome = nome;
            Apelido = apelido;
            DataNascimento = dataNascimento;
            Cc = cc;
        }
    }
}
