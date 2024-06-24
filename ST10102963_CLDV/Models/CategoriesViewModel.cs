using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ST10102963_CLDV.Models
{
    [Table("Categories")]
    public class CategoriesViewModel
    {
        [Key]
        public int CategoryID { get; set; }
       public string CategoryName { get; set; }

    }
}
