using System.ComponentModel.DataAnnotations.Schema;

namespace ATLManager.Models
{
    public class ATLAdmin
    {
        public Guid Id { get; set; }

        [ForeignKey("ATL")]
        public Guid AtlId { get; set; }
        public ATL Atl { get; set; }

        [ForeignKey("ContaAdministrativa")]
        public Guid ContaId { get; set; }
        public ContaAdministrativa ContaAdministrativa { get; set; }

        public ATLAdmin ()
        {
            Id = Guid.NewGuid();
        }
    }
}
