using System.Threading.Tasks;
using WebApplication1.Models;

namespace WebApplication1.Services
{
    public interface IOrderService
    {
        Task<int> PlaceOrderAsync(CheckoutViewModel form);
    }
}
