using Microsoft.AspNetCore.Http;
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

        public OrderService(ApplicationDbContext context, ICartService cartService, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _cartService = cartService;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<int> PlaceOrderAsync(CheckoutViewModel form)
        {
            var cartItems = await _cartService.GetCartItemsAsync();

            foreach (var item in cartItems)
            {
                if (item.Quantity > item.Product!.StockQuantity)
                    throw new InvalidOperationException(
                        $"Sorry, only {item.Product!.StockQuantity} of \"{item.Product!.Name}\" are in stock.");
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
                    Quantity = item.Quantity,
                    UnitPrice = item.Product!.Price
                });
            }

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            await _cartService.ClearCartAsync();

            return order.Id;
        }
    }
}
