using Microsoft.EntityFrameworkCore;
using Test.Models;

namespace Test.Data
{
    public class Context : DbContext
    {
        public DbSet<Movie> Movies
        {
            get;
            set;
        }

        public DbSet<Country> Country
        {
            get;
            set;
        }

        protected override void OnConfiguring (DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite ("Data Source=Data/MoviesGallery.db");
        }
    }
}