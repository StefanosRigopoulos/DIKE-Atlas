using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using API.Entities.Junctions;
using NpgsqlTypes;

namespace API.Entities
{
    public class PC
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public string Barcode { get; set; } = "";
        public string SerialNumber { get; set; } = "";
        public string Brand { get; set; } = "";
        public string Model { get; set; } = "";
        public string PCName { get; set; } = "";
        public string AdministratorCode { get; set; } = "";
        public string BIOSCode { get; set; } = "";
        public string PDFReportPath { get; set; } = "";
        public string Domain { get; set; } = "";
        public string IP { get; set; } = "";
        public string ExternalIP { get; set; } = "";
        public string SubnetMask { get; set; } = "";
        public string Gateway { get; set; } = "";
        public string DNS1 { get; set; } = "";
        public string DNS2 { get; set; } = "";

        // Navigation Properties
        public ICollection<Employee_PC> Employee_PCs { get; set; } = [];
        public ICollection<PC_CPU> PC_CPUs { get; set; } = [];
        public ICollection<PC_MOBO> PC_MOBOs { get; set; } = [];
        public ICollection<PC_RAM> PC_RAMs { get; set; } = [];
        public ICollection<PC_GPU> PC_GPUs { get; set; } = [];
        public ICollection<PC_PSU> PC_PSUs { get; set; } = [];
        public ICollection<PC_Storage> PC_Storages { get; set; } = [];
        public ICollection<PC_NetworkCard> PC_NetworkCards { get; set; } = [];
        public ICollection<PC_Monitor> PC_Monitors { get; set; } = [];
    }
}