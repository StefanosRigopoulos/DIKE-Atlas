using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using API.Entities.Junctions;

namespace API.Entities
{
    public class Monitor
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public string Barcode { get; set; } = "";
        public string SerialNumber { get; set; } = "";
        public string Brand { get; set; } = "";
        public string Model { get; set; } = "";
        public string Resolution { get; set; } = "";
        public int Inches { get; set; } = 0;
        
        // Navigation Properties
        public ICollection<PC_Monitor> PC_Monitors { get; set; } = [];
    }
}