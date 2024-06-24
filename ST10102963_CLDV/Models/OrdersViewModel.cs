using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Cryptography.Xml;

namespace ST10102963_CLDV.Models
{
    [Table("Orders")]
    public class OrdersViewModel
    {
        [Key]
        public int OrderID { get; set; }
        public DateTime OrderDate { get; set; }
        public double TotalAmount { get; set; }


        [Required]
        [ReadOnly(true)]
        public string UserEmail { get; set; }


    }
}
