namespace ATLManager.Models.DAL
{
    public class ColaboradorDAL
    {
        public int ContaColaboradores_id { get; set; }
        public int ContaUtilizador_id { get; set; }
        public string Nome { get; set; }
        public string Apelido { get; set; }
        public DateTime DataNascimento { get; set; }
        public int Cc { get; set; }

        public ColaboradorDAL(int contaColaboradores_id, int contaUtilizador_id, string nome, string apelido, DateTime dataNascimento, int cc)
        {
            ContaColaboradores_id = contaColaboradores_id;
            ContaUtilizador_id = contaUtilizador_id;
            Nome = nome;
            Apelido = apelido;
            DataNascimento = dataNascimento;
            Cc = cc;
        }
    }
}
