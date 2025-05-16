using System.Text;
using System.Text.Json;
using System.Net.Http.Headers;
using Microsoft.Extensions.Options;
using javabus_api.Settings;

public class MidtransService
{
    private readonly HttpClient _httpClient;
    private readonly MidtransSetting _settings;

    public MidtransService(IOptions<MidtransSetting> options)
    {
        _httpClient = new HttpClient();
        _settings = options.Value;
    }

    public async Task<string> CreateSnapTransactionAsync(int bookingId, int grossAmount)
    {
        var orderId = bookingId.ToString(); // Gunakan bookingId sebagai orderId Midtrans
        var authHeader = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{_settings.ServerKey}:"));

        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authHeader);

        var request = new
        {
            transaction_details = new
            {
                order_id = orderId,
                gross_amount = grossAmount
            },
            callbacks = new
            {
                finish = $"javabus://payment-success?orderId={orderId}"
            }
        };

        var content = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync("https://app.sandbox.midtrans.com/snap/v1/transactions", content);
        var responseBody = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception($"Midtrans Error: {responseBody}");
        }

        var json = JsonDocument.Parse(responseBody);
        return json.RootElement.GetProperty("redirect_url").GetString();
    }
}
