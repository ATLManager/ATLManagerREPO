using ATLManager.Areas.Identity.Data;
using ATLManager.Models;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ATLManager.ViewModels
{
    public class LowerAccountViewModel
	{
        public ATLManagerUser User { get; set; }
        public ContaAdministrativa Profile { get; set; }
        [DisplayName("Nome ATL")]
        public string AtlName { get; set; }
    }
}
