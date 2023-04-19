namespace ATLManager.ViewModels
{
    public class EstatisticasViewModel
    {
        public Dictionary<string, int> VisitasEstudoPorMesEstatisticas { get; set; }
        public Dictionary<string, int> AtividadesPorMesEstatisticas { get; set; }
        public Guid? FormularioId { get; set; }
    }
}
