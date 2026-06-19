using System.Linq;
using System.Threading.Tasks;
using WebApplication1.Data;
using WebApplication1.Models;

namespace WebApplication1.Services
{
    public class OrderService : IOrderService
    {
        private readonly ApplicationDbContext _context;
        private readonly ICartService _cartService;

        public OrderService(ApplicationDbContext context, ICartService cartService)
        {
            _context = context;
            _cartService = cartService;
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

            var order = new Order
            {
                CustomerName = form.CustomerName,
                Email = form.Email,
                ShippingAddress = form.ShippingAddress,
                TotalAmount = total,
                OrderDate = DateTime.Now
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
