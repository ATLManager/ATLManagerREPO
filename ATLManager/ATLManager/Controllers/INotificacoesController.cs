namespace ATLManager.Controllers
{
    public interface INotificacoesController
    {
        Task CreateNotification(string userId, string titulo,string mensagem);
    }
}
