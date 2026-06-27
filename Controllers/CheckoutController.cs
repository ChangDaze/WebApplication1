using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using WebApplication1.Models;
using WebApplication1.Services;

namespace WebApplication1.Controllers
{
    public class CheckoutController : Controller
    {
        private readonly IOrderService _orderService;
        private readonly ICartService _cartService;
        private readonly IPaymentService _paymentService;
        private readonly ILogger<CheckoutController> _logger;

        public CheckoutController(IOrderService orderService, ICartService cartService,
            IPaymentService paymentService, ILogger<CheckoutController> logger)
        {
            _orderService = orderService;
            _cartService = cartService;
            _paymentService = paymentService;
            _logger = logger;
        }

        // GET: /Checkout
        public async Task<IActionResult> Index()
        {
            var cartItems = await _cartService.GetCartItemsAsync();
            if (cartItems.Count == 0)
                return RedirectToAction("Index", "Cart");

            return View(new CheckoutViewModel());
        }

        // POST: /Checkout
        [HttpPost]
        public async Task<IActionResult> Index(CheckoutViewModel form)
        {
            if (!ModelState.IsValid)
                return View(form);

            // Charge the card first. Only create the order if payment succeeds.
            var amount = await _cartService.GetCartTotalAsync();
            var payment = await _paymentService.ChargeAsync(amount, form.CardNumber);
            if (!payment.Succeeded)
            {
                ModelState.AddModelError(string.Empty, payment.Error ?? "Payment failed.");
                return View(form);
            }

            try
            {
                var orderId = await _orderService.PlaceOrderAsync(form);
                return RedirectToAction("Success", new { id = orderId });
            }
            catch (InvalidOperationException ex)
            {
                // Payment already succeeded but the order could not be created.
                // This needs attention: the customer was charged without an order.
                _logger.LogError(ex,
                    "Order creation failed after a successful charge of {Amount:C} (transaction {TransactionId})",
                    amount, payment.TransactionId);
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(form);
            }
        }

        // GET: /Checkout/Success/5
        public IActionResult Success(int id)
        {
            return View(model: id);
        }
    }
}
