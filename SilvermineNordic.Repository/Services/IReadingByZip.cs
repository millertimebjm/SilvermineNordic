using System.Collections.Generic;
using System.Threading.Tasks;
using SilvermineNordic.Models;

namespace SilvermineNordic.Repository.Services;

public interface IReadingByZip
{
    Task<List<string>> GetForRefresh();
    Task<ReadingByZip?> Get(string zip);
    Task Upsert(string zip, string weatherDataSerialized);
}