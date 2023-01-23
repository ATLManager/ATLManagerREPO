namespace ATLManager.Models.DAL
{
    public class TipoContaDAL
    {
        public int tipo_conta_id { get; set; }
        public string descricao { get; set; }
        public DateTime data_criacao { get; set; }

        public TipoContaDAL(int tipo_conta_id, string descricao, DateTime data_criacao)
        {
            this.tipo_conta_id = tipo_conta_id;
            this.descricao = descricao;
            this.data_criacao = data_criacao;
        }
    }
}
