using Microsoft.CodeAnalysis;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Cryptography.Xml;

namespace ST10102963_CLDV.Models
{
    [Table("OrderDetails")]
    public class OrderDetailsViewModel
    {
        [Key]
         public int OrderDetailID { get; set; }

        [Required]
        [ForeignKey("OrdersViewModel")]
        public int OrderID { get; set; }

        [Required]
        [ForeignKey("ProductsViewModel")]
        public int ProductID { get; set; }

         public int Quantity { get; set; }

         public double UnitPrice { get; set; }

        [Required]
        [ForeignKey("OrderStatusViewModel")]
        public int OrderStatusID { get; set; }

        [Required]
        [ReadOnly(true)]
        public string UserEmail { get; set; }


        public OrdersViewModel? OrdersViewModel { get; set; }
         public ProductsViewModel? ProductsViewModel { get; set; }
         public OrderStatusViewModel? OrderStatusViewModel { get; set; }

    }
}
