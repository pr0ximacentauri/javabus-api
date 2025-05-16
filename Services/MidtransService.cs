using System.Text;
using System.Text.Json;
using System.Net.Http.Headers;
using Microsoft.Extensions.Options;
using javabus_api.Settings;

namespace javabus_api.Services
{
    public class MidtransService
    {
        private readonly HttpClient _httpClient;
        private readonly MidtransSetting _settings;

        public MidtransService(IOptions<MidtransSetting> options)
        {
            _httpClient = new HttpClient();
            _settings = options.Value;
        }

        public async Task<string> CreateTransactionAsync(int orderId, int grossAmount)
        {
            var authHeader = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{_settings.ServerKey}:"));

            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Basic", authHeader);

            var request = new
            {
                payment_type = "qris",
                transaction_details = new
                {
                    order_id = $"ORDER-{orderId}-{DateTime.UtcNow.Ticks}",
                    gross_amount = grossAmount
                },
                qris = new {}
            };

            var content = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync($"https://api.sandbox.midtrans.com/v2/charge", content);
            var responseBody = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Midtrans Error: {responseBody}");
            }

            return responseBody;
        }
    }
}
