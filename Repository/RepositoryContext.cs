using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Repository
{
    public class RepositoryContext : IdentityDbContext<User>
    {
        public RepositoryContext(DbContextOptions options) : base(options)
        {  }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<UserFolders>()
                .HasOne(x => x.User)
                .WithMany(x => x.UserFolders);

            builder.Entity<UserFolders>()
                .HasOne(x => x.Folder)
                .WithMany(x => x.UserFolders);
        }

        public DbSet<Folder> Folders { get; set; }
        public DbSet<Content> Contents { get; set; }
        public DbSet<Request> Requests { get; set; }
        public DbSet<UserFolders> UserFolders { get; set; }
        public DbSet<Backup> Backups { get; set; }
    }
}
