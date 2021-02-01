using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Test5._0.Models;

namespace Test5._0.Data
{
    public class Context : IdentityDbContext
    {
        public DbSet<Movie> Movies { get; set; }

        public DbSet<Country> Countries { get; set; }

        public DbSet<Genre> Genres { get; set; }

        /// <summary>
        /// Initializes the current context instance
        /// </summary>
        /// <param name="options">The configuration settings to be used</param>
        public Context(DbContextOptions<Context> options) : base(options) { }
    }
}