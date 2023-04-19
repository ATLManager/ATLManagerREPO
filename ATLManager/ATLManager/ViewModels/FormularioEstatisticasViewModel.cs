using ATLManager.Models;

namespace ATLManager.ViewModels
{
    public class FormularioEstatisticasViewModel
    {
        public Formulario Formulario { get; set; }
        public decimal PercentualAutorizados { get; set; }
        public decimal PercentualNaoAutorizados { get; set; }
        public int QuantidadeAutorizados { get; set; }
        public int QuantidadeNaoAutorizados { get; set; }
    }
}