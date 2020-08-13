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
        /// <summary>
        /// override OnConfiguring method of DBContext class to
        /// 1. add appsettings.json file to configuration builder
        /// 2. set connection string from appsettings to DBContext Options Builder to connect to SQL Server
        /// </summary>
        /// <param name="optionsBuilder"></param>
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            IConfigurationRoot configuration = new ConfigurationBuilder()
            .SetBasePath(Path.Combine(Directory.GetCurrentDirectory()))
            .AddJsonFile("appsettings.json")
            .Build();
            optionsBuilder.UseSqlServer(configuration.GetConnectionString("BuddyEntities"));
        }

        public DbSet<AppUser> AppUsers { set; get; }
    }
}
