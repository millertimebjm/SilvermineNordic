using SilvermineNordic.Models;
using System.Net.Http.Json;

namespace SilvermineNordic.Admin.Services
{
    public class GlobalService : IGlobalService
    {
        private User? _user { get; set; }
        public async Task<User> GetUser(HttpClient http, string apiUrl, string authKey)
        {
            if (_user is null && !string.IsNullOrWhiteSpace(authKey))
            {
                if (Guid.TryParse(authKey, out Guid authKeyGuid))
                {
                    _user = await http.GetFromJsonAsync<User>(apiUrl + "/loginauth?authkey=" + authKeyGuid.ToString());
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
