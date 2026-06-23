using System;
using System.Threading.Tasks;

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

        public Task<PaymentResult> ChargeAsync(decimal amount, string cardNumber)
        {
            var digits = (cardNumber ?? string.Empty).Replace(" ", string.Empty);

            if (digits == DeclinedCard)
                return Task.FromResult(PaymentResult.Failure("Your card was declined."));

            if (digits != ApprovedCard)
                return Task.FromResult(PaymentResult.Failure(
                    "Invalid card. Use 4242 4242 4242 4242 to simulate a successful payment."));

            // Approved: return a fake transaction id like a real gateway would
            var transactionId = "txn_" + Guid.NewGuid().ToString("N")[..16];
            return Task.FromResult(PaymentResult.Success(transactionId));
        }
    }
}
