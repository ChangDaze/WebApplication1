using System;
using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models
{
    //This represents a product that a user has added to their shopping cart but hasn't paid for yet.
    public class CartItem
    {
        [Key]
        public int Id { get; set; }
        public string SessionId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public DateTime DateCreated { get; set; } = DateTime.Now;
        
        public Product? Product { get; set; }
    }
}