using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using API.Entities.Junctions;

namespace API.Entities
{
    public class Storage
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public string Barcode { get; set; } = "";
        public string SerialNumber { get; set; } = "";
        public string Brand { get; set; } = "";
        public string Model { get; set; } = "";
        public string Type { get; set; } = "";
        public string Interface { get; set; } = "";
        public string Speed { get; set; } = "";
        public string Capacity { get; set; } = "";
        
        // Navigation Properties
        public ICollection<PC_Storage> PC_Storages { get; set; } = [];
    }
}