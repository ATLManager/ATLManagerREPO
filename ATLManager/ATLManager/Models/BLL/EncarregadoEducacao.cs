namespace ATLManager.Models.BLL
{
    public class EncarregadoEducacao
    {
        public string nome { get; set; }
        public string apelido { get; set; }
        public int telemovel { get; set; }
        public string morada { get; set; }
        public string codigo_postal { get; set; }
        public string cidade { get; set; }
        public int nif { get; set; }

        public EncarregadoEducacao(string nome, string apelido, int telemovel, string morada, string codigo_postal, string cidade, int nif)
        {
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
