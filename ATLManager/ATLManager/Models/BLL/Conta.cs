using ATLManager.Data;
using ATLManager.Models.DAL;

namespace ATLManager.Models.BLL
{
    public class Conta
    {
        
        public Conta(string email, string username, string password, int tipoContaId, int contaEstadoId, string codigoAtivacao)
        {
            //new ContaUtilizadorDAL(email, username, password, tipoContaId, contaEstadoId, codigoAtivacao);
        }

        /*  public void CriarConta(ApplicationDbContext db)
          {
              var contaDAL = new conta_utilizador(email, password, tipo_conta_id, codigo_ativacao);

              db.conta_utilizador.Add(contaDAL);
              db.SaveChanges();
           } */
      }
    }
