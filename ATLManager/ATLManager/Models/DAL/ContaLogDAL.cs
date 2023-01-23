namespace ATLManager.Models.DAL
{
    public class ContaLogDAL
    {
        public int login_id { get; set; }
        public int conta_utilizador_id { get; set; }
        public DateTime data_login { get; set; }
        public DateTime data_logout { get; set; }

        public ContaLogDAL(int login_id, int conta_utilizador_id, DateTime data_login, DateTime data_logout)
        {
            this.login_id = login_id;
            this.conta_utilizador_id = conta_utilizador_id;
            this.data_login = data_login;
            this.data_logout = data_logout;
        }
    }
}
