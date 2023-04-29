namespace ATLManager.Controllers
{

    /// <summary>
    /// Cria uma nova notificação para o utilizador especificado com o título e mensagem fornecidos.
    /// </summary>
    /// <param name="userId">O Id do utilizador que receberá a notificação.</param>
    /// <param name="titulo">O título da notificação.</param>
    /// <param name="mensagem">A mensagem da notificação.</param>
    /// <returns>Uma tarefa que representa a operação assíncrona.</returns>

    public interface INotificacoesController
    {
        Task CreateNotification(string userId, string titulo,string mensagem);
    }
}
