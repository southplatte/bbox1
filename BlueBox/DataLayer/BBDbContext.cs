using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using BlueBox.DataLayer.Models;

namespace BlueBox.DataLayer
{
    public class BBDbContext : DbContext
    {
        public BBDbContext(DbContextOptions<BBDbContext> options)
            : base(options)
        {

        }

        public DbSet<Customer> Customers { get; set; }
        public DbSet<Movie> Movies { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Shipping> Shipments { get; set; }

    }
}
