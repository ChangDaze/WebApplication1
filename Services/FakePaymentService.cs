using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace WebApplication1.Services
{
    // Development payment processor: no real money moves. Mirrors Stripe's test cards
    // so the full pay-then-order flow can be exercised, including the decline path.
    // Swap for a real StripePaymentService in production.
    public class FakePaymentService : IPaymentService
    {
        // Stripe's well-known test numbers
        private const string ApprovedCard = "4242424242424242";
        private const string DeclinedCard = "4000000000000002";

        private readonly ILogger<FakePaymentService> _logger;

        public FakePaymentService(ILogger<FakePaymentService> logger)
        {
            _logger = logger;
        }

        public Task<PaymentResult> ChargeAsync(decimal amount, string cardNumber)
        {
            var digits = (cardNumber ?? string.Empty).Replace(" ", string.Empty);

            // Never log full card numbers; the last 4 digits are safe and useful.
            var last4 = digits.Length >= 4 ? digits[^4..] : "????";

            if (digits == DeclinedCard)
            {
                _logger.LogWarning(
                    "Payment declined for {Amount:C} (card ****{Last4})", amount, last4);
                return Task.FromResult(PaymentResult.Failure("Your card was declined."));
            }

            if (digits != ApprovedCard)
            {
                _logger.LogWarning(
                    "Payment rejected: invalid card ****{Last4} for {Amount:C}", last4, amount);
                return Task.FromResult(PaymentResult.Failure(
                    "Invalid card. Use 4242 4242 4242 4242 to simulate a successful payment."));
            }

            // Approved: return a fake transaction id like a real gateway would
            var transactionId = "txn_" + Guid.NewGuid().ToString("N")[..16];
            _logger.LogInformation(
                "Payment approved for {Amount:C} (card ****{Last4}), transaction {TransactionId}",
                amount, last4, transactionId);
            return Task.FromResult(PaymentResult.Success(transactionId));
        }
    }
}
