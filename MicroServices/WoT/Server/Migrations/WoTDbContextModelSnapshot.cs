﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using WoT.Server.Services;

namespace WoT.Server.Migrations
{
    [DbContext(typeof(WoTDbContext))]
    partial class WoTDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.6-servicing-10079")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("WoT.Shared.Character", b =>
                {
                    b.Property<uint>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("varchar(32)");

                    b.Property<uint>("UserId");

                    b.HasKey("Id");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.HasIndex("UserId");

                    b.ToTable("characters");
                });

            modelBuilder.Entity("WoT.Shared.CharacterWork", b =>
                {
                    b.Property<uint>("CharacterId");

                    b.Property<bool>("IsWorking");

                    b.Property<DateTime>("StartedWorkAt");

                    b.Property<uint>("TotalHours");

                    b.Property<DateTime>("WorkFinishesAt");

                    b.HasKey("CharacterId");

                    b.ToTable("characters_work");
                });

            modelBuilder.Entity("WoT.Shared.User", b =>
                {
                    b.Property<uint>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<ulong>("DiscordId");

                    b.Property<bool>("IsPremium");

                    b.Property<ulong>("Settings");

                    b.HasKey("Id");

                    b.HasIndex("DiscordId")
                        .IsUnique();

                    b.ToTable("users");
                });

            modelBuilder.Entity("WoT.Shared.Character", b =>
                {
                    b.HasOne("WoT.Shared.User", "User")
                        .WithMany("Characters")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("WoT.Shared.CharacterWork", b =>
                {
                    b.HasOne("WoT.Shared.Character", "Character")
                        .WithOne("Work")
                        .HasForeignKey("WoT.Shared.CharacterWork", "CharacterId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
