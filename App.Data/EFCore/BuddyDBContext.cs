using App.Data.EFCore.ConceptualModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace App.Data.EFCore
{
    public class BuddyDBContext : DbContext
    {
        public BuddyDBContext()
        {

        }
        /// <summary>
        /// override OnConfiguring method of DBContext class to
        /// 1. add appsettings.json file to configuration builder
        /// 2. set connection string from appsettings to DBContext Options Builder to connect to SQL Server
        /// </summary>
        /// <param name="optionsBuilder"></param>
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            IConfigurationRoot configuration = new ConfigurationBuilder().SetBasePath(Path.Combine(Directory.GetCurrentDirectory())).AddJsonFile("appsettings.json").Build();
            optionsBuilder.UseSqlServer(configuration.GetConnectionString("BuddyEntities"));
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<InfraAccessType>().HasData(
                new { Id = 1, Name = "Admin" },
                new { Id = 2, Name = "Normal User" },
                new { Id = 3, Name = "Anonymous" }
            );

            modelBuilder.Entity<InfraCachingType>().HasData(
                new { Id = 1, Name = "No Cache" },
                new { Id = 2, Name = "Server Cache" },
                new { Id = 3, Name = "Client Cache" }
            );
        }

        public DbSet<AppUser> AppUsers { set; get; }
        public DbSet<InfraAccessType> InfraAccessType { set; get; }
        public DbSet<InfraAssembly> InfraAssembly { set; get; }
        public DbSet<InfraCachingType> InfraCachingType { set; get; }
        public DbSet<InfraClass> InfraClass { set; get; }
        public DbSet<InfraService> InfraService { set; get; }
        public DbSet<DemoMQExecution> DemoMQExecution { set; get; }
        public DbSet<DemoMQMessage> DemoMQMessage { set; get; }
    }
}
