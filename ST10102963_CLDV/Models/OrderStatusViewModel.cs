using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ST10102963_CLDV.Models
{
    [Table("OrderStatus")]
    public class OrderStatusViewModel
    {
        [Key]
        public int OrderStatusID { get; set; }
       public string StatusName {get; set;}

    }
}
