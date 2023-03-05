using System.ComponentModel.DataAnnotations;

namespace ATLManager.Models
{
    public class Agrupamento
    {
        [Key]
        public Guid AgrupamentoID { get; set; }

        [Required]
        [MaxLength(20)]
        public string Name { get; set; }

        [Required]
        [MaxLength(20)]
        public string Location { get; set; }

        public Agrupamento()
        {
            AgrupamentoID = Guid.NewGuid();
        }

        public Agrupamento(string Name, string Location) : this ()
        {
            this.Name = Name;
            this.Location = Location;
        }
    }
}
