﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Todo.Query.Infrastructure.Data;

#nullable disable

namespace Todo.Query.Infrastructure.Data.Migrations
{
    [DbContext(typeof(TodoTasksDbContext))]
    [Migration("20220716002924_CreateTasksTable")]
    partial class CreateTasksTable
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.6")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("Todo.Query.Entities.TodoTask", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("ClusterIndex")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ClusterIndex"), 1L, 1);

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("DueDate")
                        .HasColumnType("Date");

                    b.Property<bool>("IsCompleted")
                        .HasColumnType("bit");

                    b.Property<bool>("IsUniqueTitle")
                        .HasColumnType("bit");

                    b.Property<DateTime>("LastUpdate")
                        .HasColumnType("datetime2");

                    b.Property<string>("NormalizedTitle")
                        .IsRequired()
                        .HasMaxLength(148)
                        .HasColumnType("nvarchar(148)");

                    b.Property<string>("Note")
                        .HasMaxLength(1000)
                        .HasColumnType("nvarchar(1000)");

                    b.Property<int>("Sequence")
                        .HasColumnType("int");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar(128)");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar(128)");

                    b.HasKey("Id");

                    SqlServerKeyBuilderExtensions.IsClustered(b.HasKey("Id"), false);

                    b.HasIndex("ClusterIndex");

                    SqlServerIndexBuilderExtensions.IsClustered(b.HasIndex("ClusterIndex"));

                    b.HasIndex("DueDate");

                    b.HasIndex("UserId", "NormalizedTitle", "IsCompleted")
                        .IsUnique()
                        .HasFilter("[IsCompleted] = 0");

                    b.ToTable("Tasks");
                });
#pragma warning restore 612, 618
        }
    }
}
