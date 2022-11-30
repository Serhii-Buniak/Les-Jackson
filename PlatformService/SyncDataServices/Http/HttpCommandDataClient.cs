using System.Text;
using System.Text.Json;
using PlatformService.Dtos;

namespace PlatformService.SyncDataServices.Http;

public class HttpCommandDataClient : ICommandDataClient
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;

    public HttpCommandDataClient(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _configuration = configuration;
    }

    public async Task SendPlatformToCommand(PlatformReadDto platform)
    {
        const string appJson = "application/json";

        StringContent content = new(
            content: JsonSerializer.Serialize(platform),
            encoding: Encoding.UTF8,
            mediaType: appJson
        );

        HttpResponseMessage response = await _httpClient.PostAsync(_configuration["CommandService"], content);

        if (response.IsSuccessStatusCode)
        {
            Console.WriteLine("-->> HttpResponseMessage PostAsync OK");
        }
        else
        {
            Console.WriteLine("-->> HttpResponseMessage PostAsync NOT OK");
        }
    }
}