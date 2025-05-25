using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using PaymentService.Domain;

namespace PaymentService.Application.Services;

public class LiqPayService
{
    private readonly string _publicKey = "sandbox_i49912815259";
    private readonly string _privateKey = "sandbox_FpbHKZrpBNwFVyleopICCXRP8uC9S2VvUzElrj2D";

    public string CreatePaymentForm(decimal amount, string orderId, string description)
    {
        var data = new Dictionary<string, object>
        {
            { "version", 3 },
            { "public_key", _publicKey },
            { "action", "pay" },
            { "amount", amount },
            { "currency", "UAH" },
            { "description", description },
            { "order_id", orderId },
            { "server_url", "https://yourdomain/api/payment/callback" }, 
            { "result_url", "https://yourfrontend.com/payment/success" }
        };

        string json = JsonSerializer.Serialize(data);
        string base64Data = Convert.ToBase64String(Encoding.UTF8.GetBytes(json));
        string signature = GetSignature(base64Data);

        return JsonSerializer.Serialize(new
        {
            data = base64Data,
            signature
        });
    }

    private string GetSignature(string base64Data)
    {
        using var sha1 = SHA1.Create();
        var bytes = Encoding.UTF8.GetBytes(_privateKey + base64Data + _privateKey);
        var hash = sha1.ComputeHash(bytes);
        return Convert.ToBase64String(hash);
    }
}