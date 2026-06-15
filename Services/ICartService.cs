using System.Collections.Generic;
using System.Threading.Tasks;
using WebApplication1.Models;

namespace WebApplication1.Services
{
    public interface ICartService
    {
        Task AddToCartAsync(int productId, int quantity);
        Task RemoveFromCartAsync(int productId);
        Task UpdateQuantityAsync(int productId, int quantity);
        Task<List<CartItem>> GetCartItemsAsync();
        Task<decimal> GetCartTotalAsync();
        Task ClearCartAsync();
    }
}