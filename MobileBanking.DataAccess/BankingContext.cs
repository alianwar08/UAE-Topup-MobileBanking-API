using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MobileBanking.DataAccess.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MobileBanking.DataAccess
{
    public class BankingContext : DbContext
    {

        //Install-Package EntityFrameworkCore.SqlServer
        //Enable-Migrations
        //Add-Migration InitialCreate
        //Update-Database

        string conStr = "";

        public DbSet<User> Users { get; set; }
        public DbSet<Beneficiary> Beneficiaries { get; set; }
        public DbSet<TopUpTransaction> TopUpTransactions { get; set; }
        public DbSet<TopUpOption> TopUpOptions { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //// Define relationships and any additional configuration here
            //modelBuilder.Entity<User>()
            //    .HasMany(u => u.Beneficiaries)
            //    .WithOne(b => b.User)
            //    .HasForeignKey(b => b.UserID);

            //modelBuilder.Entity<User>()
            //    .HasMany(u => u.TopUpTransactions)
            //    .WithOne(t => t.User)
            //    .HasForeignKey(t => t.UserID);

            //modelBuilder.Entity<Beneficiary>()
            //    .HasMany(b => b.TopUpTransactions)
            //    .WithOne(t => t.Beneficiary)
            //    .HasForeignKey(t => t.BeneficiaryID);


            modelBuilder.Entity<TopUpOption>().HasData(
              new TopUpOption { OptionID = 1, Amount = 5M, Description = "AED 5" },
              new TopUpOption { OptionID = 2, Amount = 10M, Description = "AED 10" },
              new TopUpOption { OptionID = 3, Amount = 20M, Description = "AED 20" },
              new TopUpOption { OptionID = 4, Amount = 30M, Description = "AED 30" },
              new TopUpOption { OptionID = 5, Amount = 50M, Description = "AED 50" },
              new TopUpOption { OptionID = 6, Amount = 75M, Description = "AED 75" },
              new TopUpOption { OptionID = 7, Amount = 100M, Description = "AED 100" }
          );
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {

            var builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.json", optional: true);

            IConfigurationRoot configuration = builder.Build();
            var conStr = configuration.GetConnectionString("BankingDatabase");

            optionsBuilder.UseSqlServer(conStr);

        }

    }
}


