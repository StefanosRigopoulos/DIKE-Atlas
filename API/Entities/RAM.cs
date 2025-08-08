using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using API.Entities.Junctions;

namespace API.Entities
{
    public class RAM
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public string Barcode { get; set; } = "";
        public string SerialNumber { get; set; } = "";
        public string Brand { get; set; } = "";
        public string Model { get; set; } = "";
        public string Type { get; set; } = "";
        public string Size { get; set; } = "";
        public string Frequency { get; set; } = "";
        public string CASLatency { get; set; } = "";
        
        // Navigation Properties
        public ICollection<PC_RAM> PC_RAMs { get; set; } = [];
    }
}