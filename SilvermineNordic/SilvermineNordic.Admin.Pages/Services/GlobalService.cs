using SilvermineNordic.Models;
using System.Net.Http.Json;

namespace SilvermineNordic.Admin.Services
{
    public class GlobalService : IGlobalService
    {
        private User? _user { get; set; }
        private readonly string _apiUrl;
        public GlobalService(string apiUrl)
        {
            _apiUrl = apiUrl;
        }

        public string GetApiUrl()
        {
            return _apiUrl;
        }

        public async Task<User> GetUser(HttpClient http, string authKey)
        {
            if (_user is null && !string.IsNullOrWhiteSpace(authKey))
            {
                if (Guid.TryParse(authKey, out Guid authKeyGuid))
                {
                    _user = await http.GetFromJsonAsync<User>(_apiUrl + "/loginauth?authkey=" + authKeyGuid.ToString());
                }
            }
            return _user;
        }

        public void ClearUser()
        {
            _user = null;
        }
    }
}
