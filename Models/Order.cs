using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.Models
{
    //This represents a finalized, successful transaction
    public class Order
    {
        [Key]
        public int Id { get; set; }
        public string CustomerName { get; set; }
        public string Email { get; set; }
        public string ShippingAddress { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalAmount { get; set; }
        public DateTime OrderDate { get; set; } = DateTime.Now;
        ////order and orderdetail are mutaully owning, be aware of circular references when serializing to JSON
        public List<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
    }
}