using System.Text.Json.Serialization;

namespace backendcafe.DTO
{
    // Request DTO for creating Midtrans transaction
    public class MidtransCreateTransactionDTO
    {
        [JsonPropertyName("transaction_details")]
        public MidtransTransactionDetailsDTO TransactionDetails { get; set; }

        [JsonPropertyName("customer_details")]
        public MidtransCustomerDetailsDTO CustomerDetails { get; set; }

        [JsonPropertyName("item_details")]
        public List<MidtransItemDetailsDTO> ItemDetails { get; set; }

        [JsonPropertyName("enabled_payments")]
        public List<string> EnabledPayments { get; set; }

        [JsonPropertyName("callbacks")]
        public MidtransCallbacksDTO Callbacks { get; set; }
    }

    public class MidtransTransactionDetailsDTO
    {
        [JsonPropertyName("order_id")]
        public string OrderId { get; set; }

        [JsonPropertyName("gross_amount")]
        public decimal GrossAmount { get; set; }
    }

    public class MidtransCustomerDetailsDTO
    {
        [JsonPropertyName("first_name")]
        public string FirstName { get; set; }

        [JsonPropertyName("last_name")]
        public string LastName { get; set; }

        [JsonPropertyName("email")]
        public string Email { get; set; }

        [JsonPropertyName("phone")]
        public string Phone { get; set; }
    }

    public class MidtransItemDetailsDTO
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("price")]
        public int Price { get; set; }

        [JsonPropertyName("quantity")]
        public int Quantity { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }
    }

    public class MidtransCallbacksDTO
    {
        [JsonPropertyName("finish")]
        public string Finish { get; set; }

        [JsonPropertyName("error")]
        public string Error { get; set; }

        [JsonPropertyName("pending")]
        public string Pending { get; set; }
    }

    // Response DTO from Snap API
    public class MidtransSnapResponseDTO
    {
        [JsonPropertyName("token")]
        public string Token { get; set; }

        [JsonPropertyName("redirect_url")]
        public string RedirectUrl { get; set; }
    }

    // Transaction Status Response DTO
    public class MidtransTransactionStatusDTO
    {
        [JsonPropertyName("status_code")]
        public string StatusCode { get; set; }

        [JsonPropertyName("status_message")]
        public string StatusMessage { get; set; }

        [JsonPropertyName("transaction_id")]
        public string TransactionId { get; set; }

        [JsonPropertyName("order_id")]
        public string OrderId { get; set; }

        [JsonPropertyName("merchant_id")]
        public string MerchantId { get; set; }

        [JsonPropertyName("gross_amount")]
        public string GrossAmount { get; set; }

        [JsonPropertyName("currency")]
        public string Currency { get; set; }

        [JsonPropertyName("payment_type")]
        public string PaymentType { get; set; }

        [JsonPropertyName("transaction_time")]
        public DateTime TransactionTime { get; set; }

        [JsonPropertyName("transaction_status")]
        public string TransactionStatus { get; set; }

        [JsonPropertyName("fraud_status")]
        public string FraudStatus { get; set; }

        [JsonPropertyName("settlement_time")]
        public DateTime? SettlementTime { get; set; }
    }

    // Notification DTO from Midtrans webhook
     public class MidtransNotificationDTO
    {
        [JsonPropertyName("transaction_time")]
        public string TransactionTime { get; set; } // Changed to string for better handling

        [JsonPropertyName("transaction_status")]
        public string TransactionStatus { get; set; }

        [JsonPropertyName("transaction_id")]
        public string TransactionId { get; set; }

        [JsonPropertyName("status_message")]
        public string StatusMessage { get; set; }

        [JsonPropertyName("status_code")]
        public string StatusCode { get; set; }

        [JsonPropertyName("signature_key")]
        public string SignatureKey { get; set; }

        [JsonPropertyName("payment_type")]
        public string PaymentType { get; set; }

        [JsonPropertyName("order_id")]
        public string OrderId { get; set; }

        [JsonPropertyName("merchant_id")]
        public string MerchantId { get; set; }

        [JsonPropertyName("gross_amount")]
        public string GrossAmount { get; set; }

        [JsonPropertyName("fraud_status")]
        public string FraudStatus { get; set; }

        [JsonPropertyName("currency")]
        public string Currency { get; set; }

        [JsonPropertyName("settlement_time")]
        public string SettlementTime { get; set; } // Changed to string

        // Additional fields that might come from Midtrans
        [JsonPropertyName("transaction_type")]
        public string TransactionType { get; set; }

        [JsonPropertyName("issuer")]
        public string Issuer { get; set; }

        [JsonPropertyName("acquirer")]
        public string Acquirer { get; set; }

        [JsonPropertyName("expiry_time")]
        public string ExpiryTime { get; set; }

        // Helper methods to convert string dates to DateTime
        public DateTime? GetTransactionDateTime()
        {
            if (DateTime.TryParse(TransactionTime, out DateTime result))
                return result;
            return null;
        }

        public DateTime? GetSettlementDateTime()
        {
            if (string.IsNullOrEmpty(SettlementTime))
                return null;
            
            if (DateTime.TryParse(SettlementTime, out DateTime result))
                return result;
            return null;
        }
    }

    // DTO for creating payment request
    public class CreatePaymentRequestDTO
    {
        public int TransactionId { get; set; }
        public string CustomerName { get; set; }
        public string CustomerEmail { get; set; }
        public string CustomerPhone { get; set; }
        public List<string> EnabledPayments { get; set; } = new List<string>();
        public string FinishUrl { get; set; }
        public string ErrorUrl { get; set; }
        public string PendingUrl { get; set; }
    }

    // Response DTO for payment creation
    public class PaymentResponseDTO
    {
        public string SnapToken { get; set; }
        public string RedirectUrl { get; set; }
        public string OrderId { get; set; }
        public int TransactionId { get; set; }
    }
}