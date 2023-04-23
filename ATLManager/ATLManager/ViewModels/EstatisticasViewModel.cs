using Microsoft.Identity.Client;

namespace ATLManager.ViewModels
{
	public class EstatisticasViewModel
	{
		//Métodos do Administrador
		public Dictionary<string, int> GetAtividadesPorMesEstatisticasEnc { get; set; } // Usado no dashboard do EE
		public Dictionary<string, int> GetVisitasEstudoPorMesEstatisticasEnc { get; set; } // Usado no dashboard do EE
		public int GetNumeroDeEducandosNovosADM { get; set; }
		public int NumeroDeEducandosADM { get; set; }
		public Dictionary<string, int> EducandosPorMesADM { get; set; }
		public int NumeroDeRapazesADM { get; set; }
		public int NumeroDeRaparigasADM { get; set; }
		public int FaturasEmAtrasoADM { get; set; }
		public int FaturasPagasADM { get; set; }
		public decimal TotalValorEmAtrasoADM { get; set; }
		public decimal TotalValorPagoADM { get; set; }

		public Guid GetAgrupamentoIdFromAtl { get; set; }

		//Métodos dos Coordenadores e Funcionarios
		public Dictionary<string, int> VisitasEstudoPorMesEstatisticasCoordenadores { get; set; }
		public Dictionary<string, int> AtividadesPorMesEstatisticasCoordenadores { get; set; }
		public int NumeroDeEducandosNovos { get; set; }
		public int NumeroDeEducandos { get; set; }
		public Dictionary<string, int> EducandosPorMes { get; set; }
		public int NumeroDeRapazes { get; set; }
		public int NumeroDeRaparigas { get; set; }
		public int FaturasEmAtraso { get; set; }
		public int FaturasPagas { get; set; }
		public decimal TotalValorEmAtraso { get; set; }
		public decimal TotalValorPago { get; set; }
	}

}
