using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ST10102963_CLDV.Models;

namespace ST10102963_CLDV.Data
{
    public class ST10102963_CLDVContext : IdentityDbContext
    {
        public ST10102963_CLDVContext (DbContextOptions<ST10102963_CLDVContext> options)
            : base(options)
        {
        }


        public DbSet<ST10102963_CLDV.Models.CategoriesViewModel>? CategoriesViewModel { get; set; }

        public DbSet<ST10102963_CLDV.Models.ProductsViewModel>? ProductsViewModel { get; set; }

        public DbSet<ST10102963_CLDV.Models.OrderStatusViewModel>? OrderStatusViewModel { get; set; }

        public DbSet<ST10102963_CLDV.Models.OrderDetailsViewModel>? OrderDetailsViewModel { get; set; }

        public DbSet<ST10102963_CLDV.Models.OrdersViewModel>? OrdersViewModel { get; set; }
    }
}
