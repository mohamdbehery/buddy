﻿using App.Data.EFCore.ConceptualModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Configuration;
using System.IO;

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
            string conString = string.Empty;
            if (File.Exists(Path.Combine(Directory.GetCurrentDirectory(), "appsettings.json"))){
                Microsoft.Extensions.Configuration.IConfigurationRoot configuration = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json").Build();
                conString = configuration.GetConnectionString("BuddyEntities");
            }
            else if (File.Exists(Path.Combine(Directory.GetCurrentDirectory(), "App.config")))
            {
                conString = ConfigurationManager.ConnectionStrings["BuddyEntities"].ToString();
            }
            if (string.IsNullOrEmpty(conString)) throw new Exception("Connection String is missing");
            optionsBuilder.UseSqlServer(conString);
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
