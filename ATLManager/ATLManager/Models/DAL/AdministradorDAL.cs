namespace ATLManager.Models.DAL
{
    public class AdministradorDAL
    {
        public int administrador_id { get; set; }
        public int conta_utilizador_id { get; set; }
        public string nome { get; set; }
        public string apelido { get; set; }
        public DateTime data_nascimento { get; set; }
        public int cc { get; set; }
        public string certidao_permanente { get; set; }

        public AdministradorDAL(int administrador_id, int conta_utilizador_id, string nome, string apelido, DateTime data_nascimento, int cc, string certidao_permanente)
        {
            this.administrador_id = administrador_id;
            this.conta_utilizador_id = conta_utilizador_id;
            this.nome = nome;
            this.apelido = apelido;
            this.data_nascimento = data_nascimento;
            this.cc = cc;
            this.certidao_permanente = certidao_permanente;
        }
    }
}
