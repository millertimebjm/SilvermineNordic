using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using SilvermineNordic.Models;

namespace SilvermineNordic.Repository;

public class ZippopotamZipService : IZipApi
{
    private readonly IHttpClientFactory _httpClientFactory;
    public ZippopotamZipService(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task<ZipModelRoot> GetLatLong(ZipModelRoot model)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(model.ZipCode);
        if (model.ZipCode.Length != 5 && !int.TryParse(model.ZipCode, out _)) throw new ArgumentException("");
        var client = _httpClientFactory.CreateClient();
        var response = await client.GetAsync($"https://api.zippopotam.us/us/{model.ZipCode}");
        var json = await response.Content.ReadAsStringAsync();
        var responseModel = JsonSerializer.Deserialize<ZipModelRoot>(
            json,
            new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });
        if (responseModel is null) throw new JsonException("Zippopotam deserialize returned null");
        return responseModel;
    }
}