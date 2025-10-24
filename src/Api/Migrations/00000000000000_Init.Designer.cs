
#pragma warning disable 1591
using System;
using Api;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Api.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("00000000000000_Init")]
    partial class Init
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("ProductVersion", "8.0.7");

            modelBuilder.Entity("Api.Favorite", b =>
            {
                b.Property<int>("Id").ValueGeneratedOnAdd().HasColumnType("INTEGER");

                b.Property<DateTime>("AddedUtc").HasColumnType("TEXT");

                b.Property<string>("RecipeId").IsRequired().HasColumnType("TEXT");

                b.Property<string>("ThumbUrl").HasColumnType("TEXT");

                b.Property<string>("Title").IsRequired().HasColumnType("TEXT");

                b.HasKey("Id");

                b.ToTable("Favorites");
            });
        }
    }
}
