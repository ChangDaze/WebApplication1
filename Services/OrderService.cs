using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using WebApplication1.Data;
using WebApplication1.Models;

namespace WebApplication1.Services
{
    public class OrderService : IOrderService
    {
        private readonly ApplicationDbContext _context;
        private readonly ICartService _cartService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IEmailSender _emailSender;
        private readonly ILogger<OrderService> _logger;

        public OrderService(ApplicationDbContext context, ICartService cartService,
            IHttpContextAccessor httpContextAccessor, IEmailSender emailSender,
            ILogger<OrderService> logger)
        {
            _context = context;
            _cartService = cartService;
            _httpContextAccessor = httpContextAccessor;
            _emailSender = emailSender;
            _logger = logger;
        }

        public async Task<int> PlaceOrderAsync(CheckoutViewModel form)
        {
            var cartItems = await _cartService.GetCartItemsAsync();

            foreach (var item in cartItems)
            {
                if (item.Quantity > item.Product!.StockQuantity)
                {
                    _logger.LogWarning(
                        "Order rejected: requested {Requested} of \"{Product}\" but only {InStock} in stock",
                        item.Quantity, item.Product!.Name, item.Product!.StockQuantity);
                    throw new InvalidOperationException(
                        $"Sorry, only {item.Product!.StockQuantity} of \"{item.Product!.Name}\" are in stock.");
                }
            }

            var total = cartItems.Sum(item => item.Product!.Price * item.Quantity);

            var userId = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);

            var order = new Order
            {
                CustomerName = form.CustomerName,
                Email = form.Email,
                ShippingAddress = form.ShippingAddress,
                TotalAmount = total,
                OrderDate = DateTime.Now,
                UserId = userId
            };

            foreach (var item in cartItems)
            {
                order.OrderDetails.Add(new OrderDetail
                {
                    ProductId = item.ProductId,
                    Product = item.Product,
                    Quantity = item.Quantity,
                    UnitPrice = item.Product!.Price
                });
            }

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            _logger.LogInformation(
                "Order #{OrderId} placed by user {UserId} for {Total:C} ({ItemCount} line items)",
                order.Id, userId ?? "(guest)", total, order.OrderDetails.Count);

            await _cartService.ClearCartAsync();

            await SendConfirmationEmailAsync(order);

            return order.Id;
        }

        private async Task SendConfirmationEmailAsync(Order order)
        {
            var lines = order.OrderDetails
                .Select(d => $"  - {d.Product?.Name} x{d.Quantity} @ {d.UnitPrice:C}");

            var body =
                $"Hi {order.CustomerName},\n\n" +
                $"Thank you for your order #{order.Id} placed on {order.OrderDate:MMM dd, yyyy}.\n\n" +
                string.Join("\n", lines) + "\n\n" +
                $"Total: {order.TotalAmount:C}\n" +
                $"Shipping to: {order.ShippingAddress}\n\n" +
                "We'll let you know when it ships.";

            await _emailSender.SendEmailAsync(order.Email, $"Order Confirmation #{order.Id}", body);
        }
    }
}
