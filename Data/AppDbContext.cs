using Microsoft.EntityFrameworkCore;
using LogProxyAPI.Models;
using System.Collections.Generic;

namespace LogProxyAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Log> Logs { get; set; }
    }
}
