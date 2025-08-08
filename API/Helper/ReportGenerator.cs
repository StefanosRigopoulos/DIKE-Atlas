using API.DTOs;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace API.Helpers
{
    public class ReportGenerator
    {
        public static byte[] GeneratePDF(PCWithComponentsDTO pc)
        {
            QuestPDF.Settings.License = LicenseType.Community;
            using (var stream = new MemoryStream())
            {
                Document.Create(container =>
                {
                    container.Page(page =>
                    {
                        page.Size(PageSizes.A4);
                        page.Margin(2, Unit.Centimetre);
                        page.Background().Image(Path.Combine(Directory.GetCurrentDirectory(), "Storage", "background.png")).FitArea();
                        page.Header().Text($"Αναφορά Η/Υ - {pc.PCName}").FontSize(20).Bold().AlignCenter();
                        page.Content().Column(col =>
                        {
                            col.Item().Text("").FontSize(12);
                            col.Item().Row(row =>
                            {
                                row.RelativeItem().Column(left =>
                                {
                                    left.Item().Text("Βασικές Πληροφορίες").FontSize(16).Bold();
                                });
                                row.RelativeItem().Column(right =>
                                {
                                    right.Item().Text("Πληροφορίες Δικτύου").FontSize(16).Bold();
                                });
                            });
                            col.Item().LineHorizontal(1);
                            col.Item().Row(row =>
                            {
                                row.RelativeItem().Column(left =>
                                {
                                    left.Item().Row(r => { r.ConstantItem(130).Text("Barcode:").Bold().FontSize(11); r.RelativeItem().Text(pc.Barcode).FontSize(11); });
                                    left.Item().Row(r => { r.ConstantItem(130).Text("S/N:").Bold().FontSize(11); r.RelativeItem().Text(pc.SerialNumber).FontSize(11); });
                                    left.Item().Row(r => { r.ConstantItem(130).Text("Μάρκα:").Bold().FontSize(11); r.RelativeItem().Text(pc.Brand).FontSize(11); });
                                    left.Item().Row(r => { r.ConstantItem(130).Text("Μοντέλο:").Bold().FontSize(11); r.RelativeItem().Text(pc.Model).FontSize(11); });
                                    left.Item().Row(r => { r.ConstantItem(130).Text("Όνομα Η/Υ:").Bold().FontSize(11); r.RelativeItem().Text(pc.PCName).FontSize(11); });
                                    left.Item().Row(r => { r.ConstantItem(130).Text("Κωδικός Admin:").Bold().FontSize(11); r.RelativeItem().Text(pc.AdministratorCode).FontSize(11); });
                                    left.Item().Row(r => { r.ConstantItem(130).Text("Κωδικός BIOS:").Bold().FontSize(11); r.RelativeItem().Text(pc.BIOSCode).FontSize(11); });
                                });
                                row.RelativeItem().Column(right =>
                                {
                                    right.Item().Row(r => { r.ConstantItem(130).Text("Τομέας:").Bold().FontSize(11); r.RelativeItem().Text(pc.Domain).FontSize(11); });
                                    right.Item().Row(r => { r.ConstantItem(130).Text("Εσωτερική IP:").Bold().FontSize(11); r.RelativeItem().Text(pc.IP).FontSize(11); });
                                    right.Item().Row(r => { r.ConstantItem(130).Text("Εξωτερική IP:").Bold().FontSize(11); r.RelativeItem().Text(pc.ExternalIP).FontSize(11); });
                                    right.Item().Row(r => { r.ConstantItem(130).Text("Μάσκα:").Bold().FontSize(11); r.RelativeItem().Text(pc.SubnetMask).FontSize(11); });
                                    right.Item().Row(r => { r.ConstantItem(130).Text("Πύλη:").Bold().FontSize(11); r.RelativeItem().Text(pc.Gateway).FontSize(11); });
                                    right.Item().Row(r => { r.ConstantItem(130).Text("DNS1:").Bold().FontSize(11); r.RelativeItem().Text(pc.DNS1).FontSize(11); });
                                    right.Item().Row(r => { r.ConstantItem(130).Text("DNS2:").Bold().FontSize(11); r.RelativeItem().Text(pc.DNS2).FontSize(11); });
                                });
                            });
                            col.Item().Text("").FontSize(12);
                            col.Item().Text("Συστατικά Η/Υ").FontSize(16).Bold();
                            col.Item().LineHorizontal(1);
                            if (pc.CPUs.Any())
                            {
                                col.Item().Text("[Επεξεργαστής]").FontSize(14).Bold();
                                int i = 1;
                                foreach (var cpu in pc.CPUs)
                                {
                                    if (pc.CPUs.Count > 1) {
                                        col.Item().Text("#" + i).FontSize(12);
                                        i++;
                                    }
                                    col.Item().Row(row =>
                                    {
                                        row.RelativeItem().Column(left =>
                                        {
                                            left.Item().Row(r => { r.ConstantItem(120).Text("S/N:").Bold().FontSize(11); r.RelativeItem().Text(cpu.SerialNumber).FontSize(11); });
                                            left.Item().Row(r => { r.ConstantItem(120).Text("Μάρκα:").Bold().FontSize(11); r.RelativeItem().Text(cpu.Brand).FontSize(11); });
                                            left.Item().Row(r => { r.ConstantItem(120).Text("Μοντέλο:").Bold().FontSize(11); r.RelativeItem().Text(cpu.Model).FontSize(11); });
                                            left.Item().Row(r => { r.ConstantItem(120).Text("Πυρήνες/Νήματα:").Bold().FontSize(11); r.RelativeItem().Text(cpu.Cores.ToString() + " / " + cpu.Threads.ToString()).FontSize(11); });
                                            left.Item().Row(r => { r.ConstantItem(120).Text("Χαρακτηριστηκά:").Bold().FontSize(11); r.RelativeItem().Text(cpu.Specification).FontSize(11); });
                                        });
                                        row.RelativeItem().Column(right =>
                                        {
                                            right.Item().Row(r => { r.ConstantItem(120).Text("Barcode:").Bold().FontSize(11); r.RelativeItem().Text(cpu.Barcode).FontSize(11); });
                                            right.Item().Row(r => { r.ConstantItem(120).Text("Πακέτο:").Bold().FontSize(11); r.RelativeItem().Text(cpu.Package).FontSize(11); });
                                            right.Item().Row(r => { r.ConstantItem(120).Text("Chipset:").Bold().FontSize(11); r.RelativeItem().Text(cpu.Chipset).FontSize(11); });
                                            right.Item().Row(r => { r.ConstantItem(120).Text("Ενσωματωμένή GPU:").Bold().FontSize(11); r.RelativeItem().Text(cpu.IntegratedGPUModel).FontSize(11); });
                                        });
                                    });
                                }
                                col.Item().Text("").FontSize(14);
                            }
                            if (pc.MOBOs.Any())
                            {
                                col.Item().Text("[Μητρική Κάρτα]").FontSize(14).Bold();
                                int i = 1;
                                foreach (var mobo in pc.MOBOs)
                                {
                                    if (pc.MOBOs.Count > 1) {
                                        col.Item().Text("#" + i).FontSize(12);
                                        i++;
                                    }
                                    col.Item().Row(row =>
                                    {
                                        row.RelativeItem().Column(left =>
                                        {
                                            left.Item().Row(r => { r.ConstantItem(120).Text("S/N:").Bold().FontSize(11); r.RelativeItem().Text(mobo.SerialNumber).FontSize(11); });
                                            left.Item().Row(r => { r.ConstantItem(120).Text("Μάρκα:").Bold().FontSize(11); r.RelativeItem().Text(mobo.Brand).FontSize(11); });
                                            left.Item().Row(r => { r.ConstantItem(120).Text("Μοντέλο:").Bold().FontSize(11); r.RelativeItem().Text(mobo.Model).FontSize(11); });
                                            left.Item().Row(r => { r.ConstantItem(120).Text("Θέσεις Μνημών:").Bold().FontSize(11); r.RelativeItem().Text(mobo.DRAMSlots.ToString()).FontSize(11); });
                                            left.Item().Row(r => { r.ConstantItem(120).Text("Chipset Μάρκα").Bold().FontSize(11); r.RelativeItem().Text(mobo.ChipsetVendor).FontSize(11); });
                                            left.Item().Row(r => { r.ConstantItem(120).Text("Chipset Μοντέλο:").Bold().FontSize(11); r.RelativeItem().Text(mobo.ChipsetModel).FontSize(11); });
                                            left.Item().Row(r => { r.ConstantItem(120).Text("BIOS Μάρκα:").Bold().FontSize(11); r.RelativeItem().Text(mobo.BIOSBrand).FontSize(11); });
                                        });
                                        row.RelativeItem().Column(right =>
                                        {
                                            right.Item().Row(r => { r.ConstantItem(120).Text("Barcode:").Bold().FontSize(11); r.RelativeItem().Text(mobo.Barcode).FontSize(11); });
                                            right.Item().Row(r => { r.ConstantItem(120).Text("BIOS Έκδοση:").Bold().FontSize(11); r.RelativeItem().Text(mobo.BIOSVersion).FontSize(11); });
                                            right.Item().Row(r => { r.ConstantItem(120).Text("GPU Μάρκα:").Bold().FontSize(11); r.RelativeItem().Text(mobo.OnBoardGPUBrand).FontSize(11); });
                                            right.Item().Row(r => { r.ConstantItem(120).Text("GPU Μοντέλο:").Bold().FontSize(11); r.RelativeItem().Text(mobo.OnBoardGPUModel).FontSize(11); });
                                            right.Item().Row(r => { r.ConstantItem(120).Text("Network Μάρκα:").Bold().FontSize(11); r.RelativeItem().Text(mobo.OnBoardNetworkAdapterBrand).FontSize(11); });
                                            right.Item().Row(r => { r.ConstantItem(120).Text("Network Μοντέλο:").Bold().FontSize(11); r.RelativeItem().Text(mobo.OnBoardNetworkAdapterModel).FontSize(11); });
                                        });
                                    });
                                }
                                col.Item().Text("").FontSize(14);
                            }
                            if (pc.RAMs.Any())
                            {
                                col.Item().Text("[Μνήμη]").FontSize(14).Bold();
                                int i = 1;
                                foreach (var ram in pc.RAMs)
                                {
                                    if (pc.RAMs.Count > 1) {
                                        col.Item().Text("#" + i).FontSize(12);
                                        i++;
                                    }
                                    col.Item().Row(row =>
                                    {
                                        row.RelativeItem().Column(left =>
                                        {
                                            left.Item().Row(r => { r.ConstantItem(120).Text("S/N:").Bold().FontSize(11); r.RelativeItem().Text(ram.SerialNumber).FontSize(11); });
                                            left.Item().Row(r => { r.ConstantItem(120).Text("Μάρκα:").Bold().FontSize(11); r.RelativeItem().Text(ram.Brand).FontSize(11); });
                                            left.Item().Row(r => { r.ConstantItem(120).Text("Μοντέλο:").Bold().FontSize(11); r.RelativeItem().Text(ram.Model).FontSize(11); });
                                            left.Item().Row(r => { r.ConstantItem(120).Text("Τύπος:").Bold().FontSize(11); r.RelativeItem().Text(ram.Type).FontSize(11); });
                                        });
                                        row.RelativeItem().Column(right =>
                                        {
                                            right.Item().Row(r => { r.ConstantItem(120).Text("Barcode:").Bold().FontSize(11); r.RelativeItem().Text(ram.Barcode).FontSize(11); });
                                            right.Item().Row(r => { r.ConstantItem(120).Text("Μέγεθος:").Bold().FontSize(11); r.RelativeItem().Text(ram.Size).FontSize(11); });
                                            right.Item().Row(r => { r.ConstantItem(120).Text("Συχνότητα:").Bold().FontSize(11); r.RelativeItem().Text(ram.Frequency).FontSize(11); });
                                            right.Item().Row(r => { r.ConstantItem(120).Text("CAS Latency:").Bold().FontSize(11); r.RelativeItem().Text(ram.CASLatency).FontSize(11); });
                                        });
                                    });
                                }
                                col.Item().Text("").FontSize(14);
                            }
                            if (pc.GPUs.Any())
                            {
                                col.Item().Text("[Κάρτα Γραφικών]").FontSize(14).Bold();
                                int i = 1;
                                foreach (var gpu in pc.GPUs)
                                {
                                    if (pc.GPUs.Count > 1) {
                                        col.Item().Text("#" + i).FontSize(12);
                                        i++;
                                    }
                                    col.Item().Row(row =>
                                    {
                                        row.RelativeItem().Column(left =>
                                        {
                                            left.Item().Row(r => { r.ConstantItem(120).Text("S/N:").Bold().FontSize(11); r.RelativeItem().Text(gpu.SerialNumber).FontSize(11); });
                                            left.Item().Row(r => { r.ConstantItem(120).Text("Μάρκα:").Bold().FontSize(11); r.RelativeItem().Text(gpu.Brand).FontSize(11); });
                                            left.Item().Row(r => { r.ConstantItem(120).Text("Μοντέλο:").Bold().FontSize(11); r.RelativeItem().Text(gpu.Model).FontSize(11); });
                                        });
                                        row.RelativeItem().Column(right =>
                                        {
                                            right.Item().Row(r => { r.ConstantItem(120).Text("Barcode:").Bold().FontSize(11); r.RelativeItem().Text(gpu.Barcode).FontSize(11); });
                                            right.Item().Row(r => { r.ConstantItem(120).Text("Μνήμη:").Bold().FontSize(11); r.RelativeItem().Text(gpu.Memory).FontSize(11); });
                                            right.Item().Row(r => { r.ConstantItem(120).Text("Driver Έκδοση:").Bold().FontSize(11); r.RelativeItem().Text(gpu.DriverVersion).FontSize(11); });
                                        });
                                    });
                                }
                                col.Item().Text("").FontSize(14);
                            }
                            if (pc.PSUs.Any())
                            {
                                col.Item().Text("[Τροφοδοτικό]").FontSize(14).Bold();
                                int i = 1;
                                foreach (var psu in pc.PSUs)
                                {
                                    if (pc.PSUs.Count > 1) {
                                        col.Item().Text("#" + i).FontSize(12);
                                        i++;
                                    }
                                    col.Item().Row(row =>
                                    {
                                        row.RelativeItem().Column(left =>
                                        {
                                            left.Item().Row(r => { r.ConstantItem(120).Text("S/N:").Bold().FontSize(11); r.RelativeItem().Text(psu.SerialNumber).FontSize(11); });
                                            left.Item().Row(r => { r.ConstantItem(120).Text("Μάρκα:").Bold().FontSize(11); r.RelativeItem().Text(psu.Brand).FontSize(11); });
                                            left.Item().Row(r => { r.ConstantItem(120).Text("Μοντέλο:").Bold().FontSize(11); r.RelativeItem().Text(psu.Model).FontSize(11); });
                                        });
                                        row.RelativeItem().Column(right =>
                                        {
                                            right.Item().Row(r => { r.ConstantItem(120).Text("Barcode:").Bold().FontSize(11); r.RelativeItem().Text(psu.Barcode).FontSize(11); });
                                            right.Item().Row(r => { r.ConstantItem(120).Text("Ισχύς:").Bold().FontSize(11); r.RelativeItem().Text(psu.Wattage.ToString()).FontSize(11); });
                                            right.Item().Row(r => { r.ConstantItem(120).Text("Πιστοποίηση:").Bold().FontSize(11); r.RelativeItem().Text(psu.Certification).FontSize(11); });
                                        });
                                    });
                                }
                                col.Item().Text("").FontSize(14);
                            }
                            if (pc.Storages.Any())
                            {
                                col.Item().Text("[Αποθηκευτικό Μέσο]").FontSize(14).Bold();
                                int i = 1;
                                foreach (var storage in pc.Storages)
                                {
                                    if (pc.Storages.Count > 1) {
                                        col.Item().Text("#" + i).FontSize(12);
                                        i++;
                                    }
                                    col.Item().Row(row =>
                                    {
                                        row.RelativeItem().Column(left =>
                                        {
                                            left.Item().Row(r => { r.ConstantItem(120).Text("S/N:").Bold().FontSize(11); r.RelativeItem().Text(storage.SerialNumber).FontSize(11); });
                                            left.Item().Row(r => { r.ConstantItem(120).Text("Μάρκα:").Bold().FontSize(11); r.RelativeItem().Text(storage.Brand).FontSize(11); });
                                            left.Item().Row(r => { r.ConstantItem(120).Text("Μοντέλο:").Bold().FontSize(11); r.RelativeItem().Text(storage.Model).FontSize(11); });
                                            left.Item().Row(r => { r.ConstantItem(120).Text("Είδος:").Bold().FontSize(11); r.RelativeItem().Text(storage.Type).FontSize(11); });
                                        });
                                        row.RelativeItem().Column(right =>
                                        {
                                            right.Item().Row(r => { r.ConstantItem(120).Text("Barcode:").Bold().FontSize(11); r.RelativeItem().Text(storage.Barcode).FontSize(11); });
                                            right.Item().Row(r => { r.ConstantItem(120).Text("Διεπαφή:").Bold().FontSize(11); r.RelativeItem().Text(storage.Interface).FontSize(11); });
                                            right.Item().Row(r => { r.ConstantItem(120).Text("Ταχύτητα:").Bold().FontSize(11); r.RelativeItem().Text(storage.Speed).FontSize(11); });
                                            right.Item().Row(r => { r.ConstantItem(120).Text("Χωρητικότητα:").Bold().FontSize(11); r.RelativeItem().Text(storage.Capacity).FontSize(11); });
                                        });
                                    });
                                }
                                col.Item().Text("").FontSize(14);
                            }
                            if (pc.NetworkCards.Any())
                            {
                                col.Item().Text("[Κάρτα Δικτύου]").FontSize(14).Bold();
                                int i = 1;
                                foreach (var networkcard in pc.NetworkCards)
                                {
                                    if (pc.NetworkCards.Count > 1) {
                                        col.Item().Text("#" + i).FontSize(12);
                                        i++;
                                    }
                                    col.Item().Row(row =>
                                    {
                                        row.RelativeItem().Column(left =>
                                        {
                                            left.Item().Row(r => { r.ConstantItem(120).Text("S/N:").Bold().FontSize(11); r.RelativeItem().Text(networkcard.SerialNumber).FontSize(11); });
                                            left.Item().Row(r => { r.ConstantItem(120).Text("Μάρκα:").Bold().FontSize(11); r.RelativeItem().Text(networkcard.Brand).FontSize(11); });
                                        });
                                        row.RelativeItem().Column(right =>
                                        {
                                            right.Item().Row(r => { r.ConstantItem(120).Text("Barcode:").Bold().FontSize(11); r.RelativeItem().Text(networkcard.Barcode).FontSize(11); });
                                            right.Item().Row(r => { r.ConstantItem(120).Text("Μοντέλο:").Bold().FontSize(11); r.RelativeItem().Text(networkcard.Model).FontSize(11); });
                                        });
                                    });
                                }
                                col.Item().Text("").FontSize(14);
                            }
                            if (pc.Monitors.Any())
                            {
                                col.Item().Text("[Οθόνη]").FontSize(14).Bold();
                                int i = 1;
                                foreach (var monitor in pc.Monitors)
                                {
                                    if (pc.Monitors.Count > 1) {
                                        col.Item().Text("#" + i).FontSize(12);
                                        i++;
                                    }
                                    col.Item().Row(row =>
                                    {
                                        row.RelativeItem().Column(left =>
                                        {
                                            left.Item().Row(r => { r.ConstantItem(120).Text("S/N:").Bold().FontSize(11); r.RelativeItem().Text(monitor.SerialNumber).FontSize(11); });
                                            left.Item().Row(r => { r.ConstantItem(120).Text("Μάρκα:").Bold().FontSize(11); r.RelativeItem().Text(monitor.Brand).FontSize(11); });
                                            left.Item().Row(r => { r.ConstantItem(120).Text("Μοντέλο:").Bold().FontSize(11); r.RelativeItem().Text(monitor.Model).FontSize(11); });
                                        });
                                        row.RelativeItem().Column(right =>
                                        {
                                            right.Item().Row(r => { r.ConstantItem(120).Text("Barcode:").Bold().FontSize(11); r.RelativeItem().Text(monitor.Barcode).FontSize(11); });
                                            right.Item().Row(r => { r.ConstantItem(120).Text("Ανάλυση:").Bold().FontSize(11); r.RelativeItem().Text(monitor.Resolution).FontSize(11); });
                                            right.Item().Row(r => { r.ConstantItem(120).Text("Ίντσες:").Bold().FontSize(11); r.RelativeItem().Text(monitor.Inches.ToString()).FontSize(11); });
                                        });
                                    });
                                }
                                col.Item().Text("").FontSize(14);
                            }
                        });
                        page.Footer().Column(col =>
                        {
                            col.Item().LineHorizontal(1);
                            col.Item().Text("DIKE Atlas Report").FontSize(8).Italic().AlignCenter();
                        });
                    });
                }).GeneratePdf(stream);
                return stream.ToArray();
            }
        }
        public static byte[] GenerateEmployeeCard(EmployeeDTO employee)
        {
            QuestPDF.Settings.License = LicenseType.Community;
            QuestPDF.Settings.EnableDebugging = true;

            using (var stream = new MemoryStream())
            {
                Document.Create(container =>
                {
                    container.Page(page =>
                    {
                        page.Size(80, 50, Unit.Millimetre);
                        page.Margin(2, Unit.Millimetre);
                        page.Header().Text($"{employee.Rank} ({employee.Speciality}) {employee.FirstName} {employee.LastName}".ToUpper())
                            .FontFamily("Roboto").FontSize(10).Bold().AlignCenter();
                        page.Content().Row(row =>
                        {
                            row.RelativeItem(2).Padding(2, Unit.Millimetre).AlignCenter().AlignMiddle().Column(leftCol => {
                                leftCol.Item().Border(1).BorderColor(Colors.Grey.Lighten2).Background(Colors.White).AlignMiddle().Image(employee.PhotoPath).FitArea();
                                leftCol.Item().Text($"{employee.Rank}({employee.Speciality})")
                                    .FontSize(8)
                                    .Bold()
                                    .AlignCenter();
                            });
                            row.RelativeItem(3).AlignMiddle().Column(col => {
                                col.Item().Row(infoRow => {
                                    //infoRow.ConstantItem(8).Image("icons/id-card.png").FitWidth();
                                    infoRow.RelativeItem().Text($" 🪪  {employee.AM}").FontSize(8);
                                });
                                col.Item().Row(infoRow => {
                                    //infoRow.ConstantItem(8).Image("icons/phone.png").FitWidth(); // Phone icon
                                    infoRow.RelativeItem().Text($" 📞  271039{employee.Phone}").FontSize(8);
                                });
                                col.Item().Row(infoRow => {
                                    //infoRow.ConstantItem(8).Image("icons/mobile.png").FitWidth(); // Mobile icon
                                    infoRow.RelativeItem().Text($" 📱  {employee.Mobile}").FontSize(8);
                                });
                                col.Item().Row(infoRow => {
                                    //infoRow.ConstantItem(8).Image("icons/email.png").FitWidth(); // Email icon
                                    infoRow.RelativeItem().Text($" 📧  {employee.Email}").FontSize(8);
                                });
                                col.Item().LineHorizontal(1).LineColor(Colors.Grey.Lighten2);
                                col.Item().Row(infoRow => {
                                    //infoRow.ConstantItem(8).Image("icons/unit.png").FitWidth(); // Unit icon
                                    infoRow.RelativeItem().Text($" 🏢  {employee.Unit}").FontSize(8);
                                });
                                col.Item().Row(infoRow => {
                                    //infoRow.ConstantItem(8).Image("icons/office.png").FitWidth(); // Office icon
                                    infoRow.RelativeItem().Text($" 📌  {employee.Office}").FontSize(8);
                                });
                                col.Item().Row(infoRow => {
                                    //infoRow.ConstantItem(8).Image("icons/position.png").FitWidth(); // Position icon
                                    infoRow.RelativeItem().Text($" 📍  {employee.Position}").FontSize(8);
                                });
                            });
                        });
                    });
                }).GeneratePdf(stream);
                return stream.ToArray();
            }
        }
    }
}