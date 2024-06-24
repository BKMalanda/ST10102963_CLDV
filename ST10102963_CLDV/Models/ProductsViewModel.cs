using Humanizer;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.CodeAnalysis;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ST10102963_CLDV.Models
{
    [Table("Products")]
    public class ProductsViewModel
    {
        [Key]
        public int ProductID { get; set; }
        public string ProductName { get; set; }
        public double Price  { get; set; }

        [Required]
        [ForeignKey("CategoriesViewModel")]
        public int CategoryID { get; set; }
        public string Availability  { get; set; }

        public CategoriesViewModel? CategoriesViewModel { get; set; }

    }
}
