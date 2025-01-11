﻿using Microsoft.EntityFrameworkCore;
using MiniETicaret.Products.WebAPI.Models;

namespace MiniETicaret.Products.WebAPI.Context;


public sealed class AplicationDbContext: DbContext
{
    public AplicationDbContext(DbContextOptions options) : base(options)
    {
    }
    public DbSet<Product> Products { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>(builder =>
        {
            builder.Property(p => p.Price).HasColumnType("money");
        });
    }
}
