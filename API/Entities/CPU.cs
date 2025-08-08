using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using API.Entities.Junctions;

namespace API.Entities
{
    public class CPU
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public string Barcode { get; set; } = "";
        public string SerialNumber { get; set; } = "";
        public string Brand { get; set; } = "";
        public string Model { get; set; } = "";
        public int Cores { get; set; } = 0;
        public int Threads { get; set; } = 0;
        public string Specification { get; set; } = "";
        public string Package { get; set; } = "";
        public string Chipset { get; set; } = "";
        public string IntegratedGPUModel { get; set; } = "";
        
        // Navigation Properties
        public ICollection<PC_CPU> PC_CPUs { get; set; } = [];
    }
}