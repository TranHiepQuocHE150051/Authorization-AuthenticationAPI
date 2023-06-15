using Microsoft.EntityFrameworkCore;
using RestfulAPICRUDExample.Models;

namespace RestfulAPICRUDExample.Data
{
    public class ApplicationDbContext:DbContext 
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
        public DbSet<Villa> Villas { get; set; }
        public DbSet<LocalUser> LocalUsers { get; set; }
    }
}
