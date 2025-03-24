using Microsoft.EntityFrameworkCore;
using static System.Console;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkinfWithEFCore.repo.model;

namespace WorkinfWithEFCore.repo
{
    public class Northwind : DbContext
    {
        public DbSet<Category>? Categories { get; set; }
        public DbSet<Product>? Products { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string path = Path.Combine(Environment.CurrentDirectory, "NorthWind.db"); 
            WriteLine($"Path: {path}");
            optionsBuilder.UseSqlite($"Filename={path}");
            optionsBuilder.UseLazyLoadingProxies();
            //base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Category>()
                .Property(category => category.CategoryName)
                .IsRequired()
                .HasMaxLength(15);
            modelBuilder.Entity<Product>()
                .HasQueryFilter(p => !p.Discontinued);
            modelBuilder.Entity<Product>() 
                .Property(product => product.Cost)
                .HasConversion<double>();
            //base.OnModelCreating(modelBuilder);
        }
    }
}
