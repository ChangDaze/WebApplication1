using System.Threading.Tasks;

namespace WebApplication1.Services
{
    public class PaymentResult
    {
        public bool Succeeded { get; set; }
        public string? TransactionId { get; set; }
        public string? Error { get; set; }

        public static PaymentResult Success(string transactionId) =>
            new PaymentResult { Succeeded = true, TransactionId = transactionId };

        public static PaymentResult Failure(string error) =>
            new PaymentResult { Succeeded = false, Error = error };
    }

    public interface IPaymentService
    {
        Task<PaymentResult> ChargeAsync(decimal amount, string cardNumber);
    }
}
