﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Zabr.Craiglists.Crawler.Data;
using Zabr.Crawler.Data;

#nullable disable

namespace Zabr.Craiglists.Crawler.Data.Migrations
{
    [DbContext(typeof(DataDbContext))]
    partial class CraiglistsContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.14")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("Zabr.Craiglists.Crawler.Data.Entities.PageEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<DateTime>("Date")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Response")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("Url")
                        .IsRequired()
                        .HasMaxLength(250)
                        .HasColumnType("NVARCHAR");

                    b.HasKey("Id");

                    b.HasIndex(new[] { "Url" }, "IX_URL")
                        .IsUnique();

                    b.ToTable("Pages");
                });
#pragma warning restore 612, 618
        }
    }
}
