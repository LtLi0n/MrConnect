﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace WoT.Server.Migrations
{
    [DbContext(typeof(WoTDbContext))]
    [Migration("20191010220022_CharacterWorkInitial")]
    partial class CharacterWorkInitial
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
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

                    b.OwnsOne("WoT.Shared.Character_Skills", "Skills", b1 =>
                        {
                            b1.Property<uint>("CharacterId");

                            b1.Property<ulong>("MiningXp")
                                .HasColumnName("mining_xp");

                            b1.Property<ulong>("WoodcuttingXp")
                                .HasColumnName("woodcutting_xp");

                            b1.HasKey("CharacterId");

                            b1.ToTable("characters");

                            b1.HasOne("WoT.Shared.Character")
                                .WithOne("Skills")
                                .HasForeignKey("WoT.Shared.Character_Skills", "CharacterId")
                                .OnDelete(DeleteBehavior.Cascade);
                        });
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
