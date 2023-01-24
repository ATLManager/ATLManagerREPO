namespace ATLManager.Models.DAL
{
    public class EncarregadoEducacaoDAL
    {
        public int encarregado_educacao_id { get; set; }
        public int conta_utilizador_id { get; set; }
        public string nome { get; set; }
        public string apelido { get; set; }
        public int telemovel { get; set; }
        public string morada { get; set; }
        public string codigo_postal { get; set; }
        public string cidade { get; set; }
        public int nif { get; set; }

        public EncarregadoEducacaoDAL(int encarregado_educacao_id, int conta_utilizador_id, string nome, string apelido, int telemovel, string morada, string codigo_postal, string cidade, int nif)
        {
            this.encarregado_educacao_id = encarregado_educacao_id;
            this.conta_utilizador_id = conta_utilizador_id;
            this.nome = nome;
            this.apelido = apelido;
            this.telemovel = telemovel;
            this.morada = morada;
            this.codigo_postal = codigo_postal;
            this.cidade = cidade;
            this.nif = nif;
        }
    }
}
