namespace ATLManager.ViewModels
{
    public class HomeEstatisticasViewModel
    {
        public Dictionary<string, int> VisitasEstudoPorMesEstatisticasCoordenadores { get; set; }
        public Dictionary<string, int> AtividadesPorMesEstatisticasCoordenadores { get; set; }
        public Dictionary<string, int> VisitasEstudoPorMesEstatisticasEncarregados { get; set; }
        public Dictionary<string, int> AtividadesPorMesEstatisticasEncarregados { get; set; }
    }
}
