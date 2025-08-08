using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using API.Entities.Junctions;

namespace API.Entities
{
    public class NetworkCard
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public string Barcode { get; set; } = "";
        public string SerialNumber { get; set; } = "";
        public string Brand { get; set; } = "";
        public string Model { get; set; } = "";
        
        // Navigation Properties
        public ICollection<PC_NetworkCard> PC_NetworkCards { get; set; } = [];
    }
}