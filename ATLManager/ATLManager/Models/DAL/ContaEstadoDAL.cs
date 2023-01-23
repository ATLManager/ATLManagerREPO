namespace ATLManager.Models.DAL
{
    public class ContaEstadoDAL
    {
        public int conta_estado_id { get; set; }
        public string descricao { get; set; }
        public DateTime data_criacao { get; set; }

        public ContaEstadoDAL(int conta_estado_id, string descricao, DateTime data_criacao)
        {
            this.conta_estado_id = conta_estado_id;
            this.descricao = descricao;
            this.data_criacao = data_criacao;
        }
    }
}
