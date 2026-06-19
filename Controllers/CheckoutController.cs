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

        public CheckoutController(IOrderService orderService, ICartService cartService)
        {
            _orderService = orderService;
            _cartService = cartService;
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

            try
            {
                var orderId = await _orderService.PlaceOrderAsync(form);
                return RedirectToAction("Success", new { id = orderId });
            }
            catch (InvalidOperationException ex)
            {
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
