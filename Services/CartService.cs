using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication1.Data;
using WebApplication1.Models;

namespace WebApplication1.Services
{
    public class CartService : ICartService
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private const string CartSessionKey = "CartId";

        public CartService(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        // Helper to securely get or generate a Session ID for the guest/user
        private string GetCartId()
        {
            var session = _httpContextAccessor.HttpContext?.Session;
            if (session == null)
                throw new InvalidOperationException("Session is not configured properly.");

            var cartId = session.GetString(CartSessionKey);
            if (string.IsNullOrEmpty(cartId))
            {
                cartId = Guid.NewGuid().ToString();
                session.SetString(CartSessionKey, cartId);
            }

            return cartId;
        }

        public async Task AddToCartAsync(int productId, int quantity)
        {
            var cartId = GetCartId();
            var cartItem = await _context.CartItems
                .SingleOrDefaultAsync(c => c.SessionId == cartId && c.ProductId == productId);

            if (cartItem == null)
            {
                cartItem = new CartItem
                {
                    SessionId = cartId,
                    ProductId = productId,
                    Quantity = quantity
                };
                _context.CartItems.Add(cartItem);
            }
            else
            {
                cartItem.Quantity += quantity;
            }

            await _context.SaveChangesAsync();
        }

        public async Task RemoveFromCartAsync(int productId)
        {
            var cartId = GetCartId();
            var cartItem = await _context.CartItems
                .SingleOrDefaultAsync(c => c.SessionId == cartId && c.ProductId == productId);

            if (cartItem != null)
            {
                _context.CartItems.Remove(cartItem);
                await _context.SaveChangesAsync();
            }
        }

        public async Task UpdateQuantityAsync(int productId, int quantity)
        {
            var cartId = GetCartId();
            var cartItem = await _context.CartItems
                .SingleOrDefaultAsync(c => c.SessionId == cartId && c.ProductId == productId);

            if (cartItem != null)
            {
                if (quantity > 0)
                {
                    cartItem.Quantity = quantity;
                }
                else
                {
                    _context.CartItems.Remove(cartItem);
                }
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<CartItem>> GetCartItemsAsync()
        {
            var cartId = GetCartId();
            return await _context.CartItems
                .Include(c => c.Product)
                .Where(c => c.SessionId == cartId)
                .ToListAsync();
        }

        public async Task<decimal> GetCartTotalAsync()
        {
            var cartId = GetCartId();
            return await _context.CartItems
                .Where(c => c.SessionId == cartId)
                // We multiply current product price by quantity securely from the server
                .Select(c => c.Product!.Price * c.Quantity) 
                .SumAsync();
        }

        public async Task ClearCartAsync()
        {
            var cartId = GetCartId();
            var cartItems = _context.CartItems.Where(c => c.SessionId == cartId);
            _context.CartItems.RemoveRange(cartItems);
            await _context.SaveChangesAsync();
        }
    }
}