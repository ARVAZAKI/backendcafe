using System.Text;
using System.Text.Json;
using backendcafe.DTO;

namespace backendcafe.Services
{
    public interface IMidtransService
    {
        Task<MidtransSnapResponseDTO> CreateSnapTokenAsync(MidtransCreateTransactionDTO request);
        Task<MidtransTransactionStatusDTO> GetTransactionStatusAsync(string orderId);
        Task<bool> VerifyNotificationAsync(MidtransNotificationDTO notification);
    }

    public class MidtransService : IMidtransService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly string _serverKey;
        private readonly string _baseUrl;

        public MidtransService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _serverKey = _configuration["Midtrans:ServerKey"];
            _baseUrl = _configuration["Midtrans:IsProduction"] == "true" 
                ? "https://api.midtrans.com/v2" 
                : "https://api.sandbox.midtrans.com/v2";

            // Setup HttpClient with authorization header
            var authValue = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{_serverKey}:"));
            _httpClient.DefaultRequestHeaders.Authorization = 
                new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", authValue);
            _httpClient.DefaultRequestHeaders.Accept.Add(
                new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
        }

        public async Task<MidtransSnapResponseDTO> CreateSnapTokenAsync(MidtransCreateTransactionDTO request)
        {
            try
            {
                var snapUrl = _configuration["Midtrans:IsProduction"] == "true" 
                    ? "https://app.midtrans.com/snap/v1/transactions" 
                    : "https://app.sandbox.midtrans.com/snap/v1/transactions";

                var json = JsonSerializer.Serialize(request, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
                });

                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync(snapUrl, content);

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var snapResponse = JsonSerializer.Deserialize<MidtransSnapResponseDTO>(responseContent, 
                        new JsonSerializerOptions
                        {
                            PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
                        });
                    return snapResponse;
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    throw new Exception($"Midtrans API Error: {response.StatusCode} - {errorContent}");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to create Snap token: {ex.Message}");
            }
        }

        public async Task<MidtransTransactionStatusDTO> GetTransactionStatusAsync(string orderId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_baseUrl}/{orderId}/status");
                
                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var statusResponse = JsonSerializer.Deserialize<MidtransTransactionStatusDTO>(responseContent,
                        new JsonSerializerOptions
                        {
                            PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
                        });
                    return statusResponse;
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    throw new Exception($"Midtrans API Error: {response.StatusCode} - {errorContent}");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to get transaction status: {ex.Message}");
            }
        }

        public async Task<bool> VerifyNotificationAsync(MidtransNotificationDTO notification)
        {
            try
            {
                // Verify signature hash (optional but recommended for security)
                var signatureKey = _configuration["Midtrans:ServerKey"];
                var orderId = notification.OrderId;
                var statusCode = notification.StatusCode;
                var grossAmount = notification.GrossAmount;
                
                var input = $"{orderId}{statusCode}{grossAmount}{signatureKey}";
                var hash = System.Security.Cryptography.SHA512.HashData(Encoding.UTF8.GetBytes(input));
                var hashString = Convert.ToHexString(hash).ToLower();

                return hashString == notification.SignatureKey?.ToLower();
            }
            catch
            {
                return false;
            }
        }
    }
}