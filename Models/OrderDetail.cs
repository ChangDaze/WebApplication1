using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.Models
{
    //These are the individual items attached to a specific finalized Order (the lines on a receipt).
    public class OrderDetail
    {
        [Key]
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal UnitPrice { get; set; }
        //order and orderdetail are mutaully owning, be aware of circular references when serializing to JSON
        public Order? Order { get; set; }
        public Product? Product { get; set; }
    }
}