﻿// <auto-generated />
using System;
using ClubService.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace ClubService.Infrastructure.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20240430152928_Initial")]
    partial class Initial
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.2")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("ClubService.Domain.Event.DomainEnvelope<ClubService.Domain.Event.IDomainEvent>", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<Guid>("EntityId")
                        .HasColumnType("uuid")
                        .HasColumnName("entityId");

                    b.Property<string>("EntityType")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("entityType");

                    b.Property<string>("EventData")
                        .HasColumnType("text")
                        .HasColumnName("eventData");

                    b.Property<Guid>("EventId")
                        .HasColumnType("uuid")
                        .HasColumnName("eventId");

                    b.Property<string>("EventType")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("eventType");

                    b.Property<DateTime>("Timestamp")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("timestamp");

                    b.HasKey("Id")
                        .HasName("pK_DomainEvent");

                    b.ToTable("DomainEvent", (string)null);
                });
#pragma warning restore 612, 618
        }
    }
}