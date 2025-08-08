using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using API.Entities.Junctions;

namespace API.Entities
{
    public class MOBO
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public string Barcode { get; set; } = "";
        public string SerialNumber { get; set; } = "";
        public string Brand { get; set; } = "";
        public string Model { get; set; } = "";
        public int DRAMSlots { get; set; }
        public string ChipsetVendor { get; set; } = "";
        public string ChipsetModel { get; set; } = "";
        public string BIOSBrand { get; set; } = "";
        public string BIOSVersion { get; set; } = "";
        public string OnBoardGPUBrand { get; set; } = "";
        public string OnBoardGPUModel { get; set; } = "";
        public string OnBoardNetworkAdapterBrand { get; set; } = "";
        public string OnBoardNetworkAdapterModel { get; set; } = "";
        
        // Navigation Properties
        public ICollection<PC_MOBO> PC_MOBOs { get; set; } = [];
    }
}