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
        DbSet<Customer> Cusomters { get; set; }
        DbSet<Identity> Identities { get; set; }
        DbSet<Movie> Movies { get; set; }
        DbSet<Payment> Payments { get; set; }
        DbSet<Shipping> Shipments { get; set; }

    }
}
