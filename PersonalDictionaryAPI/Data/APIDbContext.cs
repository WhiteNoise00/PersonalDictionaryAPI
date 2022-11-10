using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PersonalDictionaryAPI.Models;
using System.Collections.Generic;

namespace PersonalDictionaryAPI.Data
{
    public class APIDbContext : DbContext
    {
        public DbSet<Category> Categories { get; set; }
        public DbSet<Note> Notes { get; set; }
        public DbSet<Language> Languages { get; set; }
        public APIDbContext(DbContextOptions<APIDbContext> options)
            : base(options)
        {}
    }
}
