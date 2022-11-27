using SilvermineNordic.Models;

namespace SilvermineNordic.Admin.Services
{
    public interface IGlobalService
    {
        Task<User> GetUser(HttpClient http, string authKey);
        void ClearUser();
        string GetApiUrl();
    }
}
