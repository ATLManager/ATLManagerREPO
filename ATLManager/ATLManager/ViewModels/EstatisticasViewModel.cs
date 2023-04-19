namespace ATLManager.ViewModels
{
	public class EstatisticasViewModel
	{
		public Dictionary<string, int> VisitasEstudoPorMesEstatisticas { get; set; }
		public Dictionary<string, int> AtividadesPorMesEstatisticas { get; set; }
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
