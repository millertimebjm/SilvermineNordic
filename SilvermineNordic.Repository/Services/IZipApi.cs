using System.Threading.Tasks;
using SilvermineNordic.Models;

namespace SilvermineNordic.Repository;

public interface IZipApi
{
    Task<ZipModelRoot> GetLatLong(ZipModelRoot model);
}