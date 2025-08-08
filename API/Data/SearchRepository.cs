using API.DTOs;
using API.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class SearchRepository(DataContext context) : ISearchRepository
    {
        public async Task<IEnumerable<SearchResultDTO>> SearchAsync(string searchTerm)
        {
            var queryText = searchTerm + ":*";
            var employeeQuery =
                from e in context.Employees
                where EF.Functions.ToTsVector("greek", e.AM + " " + e.Rank + " " + e.Speciality + " " + e.FirstName + " " + e.LastName + " " + e.Unit + " " + e.Office + " " + e.Position + " " + e.Phone + " " + e.Mobile + " " + e.Email)
                        .Matches(EF.Functions.PhraseToTsQuery("greek", queryText)) ||
                    EF.Functions.ToTsVector("english", e.AM + " " + e.Rank + " " + e.Speciality + " " + e.FirstName + " " + e.LastName + " " + e.Unit + " " + e.Office + " " + e.Position + " " + e.Phone + " " + e.Mobile + " " + e.Email)
                        .Matches(EF.Functions.PhraseToTsQuery("english", queryText))
                select new SearchResultDTO
                {
                    Type = "Employee",
                    ID = e.ID,
                    Title = e.Rank + " (" + e.Speciality + ") " + e.LastName + " " + e.FirstName,
                    Description = e.Unit + " " + e.Office + " " + e.Position
                };
            var pcQuery =
                from p in context.PCs
                where EF.Functions.ToTsVector("english", p.Barcode + " " + p.Brand + " " + p.Model + " " + p.IP + " " + p.ExternalIP)
                        .Matches(EF.Functions.PhraseToTsQuery("english", queryText))
                select new SearchResultDTO
                {
                    Type = "PC",
                    ID = p.ID,
                    Title = p.Brand + " " + p.Model,
                    Description = p.Barcode + " (IP: " + p.IP + ")"
                };
            var cpuQuery =
                from c in context.CPUs
                where EF.Functions.ToTsVector("english", c.Barcode + " " + c.Brand + " " + c.Model)
                        .Matches(EF.Functions.PhraseToTsQuery("english", queryText))
                select new SearchResultDTO
                {
                    Type = "CPU",
                    ID = c.ID,
                    Title = c.Brand + " " + c.Model,
                    Description = c.Barcode
                };
            var moboQuery =
                from m in context.MOBOs
                where EF.Functions.ToTsVector("english", m.Barcode + " " + m.Brand + " " + m.Model)
                        .Matches(EF.Functions.PhraseToTsQuery("english", queryText))
                select new SearchResultDTO
                {
                    Type = "MOBO",
                    ID = m.ID,
                    Title = m.Brand + " " + m.Model,
                    Description = m.Barcode
                };
            var ramQuery =
                from r in context.RAMs
                where EF.Functions.ToTsVector("english", r.Barcode + " " + r.Brand + " " + r.Model)
                        .Matches(EF.Functions.PhraseToTsQuery("english", queryText))
                select new SearchResultDTO
                {
                    Type = "RAM",
                    ID = r.ID,
                    Title = r.Brand + " " + r.Model,
                    Description = r.Barcode + " (Type: " + r.Type + ")"
                };
            var gpuQuery =
                from g in context.GPUs
                where EF.Functions.ToTsVector("english", g.Barcode + " " + g.Brand + " " + g.Model)
                        .Matches(EF.Functions.PhraseToTsQuery("english", queryText))
                select new SearchResultDTO
                {
                    Type = "GPU",
                    ID = g.ID,
                    Title = g.Brand + " " + g.Model,
                    Description = g.Barcode
                };
            var psuQuery =
                from p in context.PSUs
                where EF.Functions.ToTsVector("english", p.Barcode + " " + p.Brand + " " + p.Model)
                        .Matches(EF.Functions.PhraseToTsQuery("english", queryText))
                select new SearchResultDTO
                {
                    Type = "PSU",
                    ID = p.ID,
                    Title = p.Brand + " " + p.Model,
                    Description = p.Barcode
                };
            var storageQuery =
                from s in context.Storages
                where EF.Functions.ToTsVector("english", s.Barcode + " " + s.Brand + " " + s.Model)
                        .Matches(EF.Functions.PhraseToTsQuery("english", queryText))
                select new SearchResultDTO
                {
                    Type = "Storage",
                    ID = s.ID,
                    Title = s.Brand + " " + s.Model,
                    Description = s.Barcode + " (Type: " + s.Type + ")"
                };
            var networkcardQuery =
                from n in context.NetworkCards
                where EF.Functions.ToTsVector("english", n.Barcode + " " + n.Brand + " " + n.Model)
                        .Matches(EF.Functions.PhraseToTsQuery("english", queryText))
                select new SearchResultDTO
                {
                    Type = "NetworkCard",
                    ID = n.ID,
                    Title = n.Brand + " " + n.Model,
                    Description = n.Barcode
                };
            var monitorQuery =
                from m in context.Monitors
                where EF.Functions.ToTsVector("english", m.Barcode + " " + m.Brand + " " + m.Model)
                        .Matches(EF.Functions.PhraseToTsQuery("english", queryText))
                select new SearchResultDTO
                {
                    Type = "Monitor",
                    ID = m.ID,
                    Title = m.Brand + " " + m.Model,
                    Description = m.Barcode
                };
            var results = await employeeQuery
                .Concat(pcQuery)
                .Concat(cpuQuery)
                .Concat(moboQuery)
                .Concat(ramQuery)
                .Concat(gpuQuery)
                .Concat(psuQuery)
                .Concat(storageQuery)
                .Concat(networkcardQuery)
                .Concat(monitorQuery)
                .ToListAsync();
            return results;
        }
    }
}