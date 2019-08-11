﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using ServerDiscord.Services;

namespace ServerDiscord.Migrations
{
    [DbContext(typeof(DataContext))]
    partial class DataContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.6-servicing-10079")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("SharedDiscord.GuildEmote", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<ulong>("GuildId");

                    b.Property<bool>("IsAnimated")
                        .HasColumnName("animated?");

                    b.Property<bool>("IsManaged");

                    b.Property<string>("Name")
                        .IsRequired();

                    b.Property<bool>("RequireColons");

                    b.Property<ulong?>("UserId");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("guild_emote");
                });

            modelBuilder.Entity("SharedDiscord.Message", b =>
                {
                    b.Property<ulong>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<ulong>("ChannelId");

                    b.Property<string>("Content")
                        .IsRequired()
                        .HasMaxLength(2000);

                    b.Property<ulong?>("GuildId");

                    b.Property<DateTime>("TimeStamp");

                    b.Property<ulong?>("UserId");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("message");
                });

            modelBuilder.Entity("SharedDiscord.MessageEdit", b =>
                {
                    b.Property<ulong>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Content")
                        .HasMaxLength(2000);

                    b.Property<ulong>("MessageId");

                    b.Property<DateTime>("TimeStamp");

                    b.HasKey("Id");

                    b.HasIndex("MessageId");

                    b.ToTable("message_edit");
                });

            modelBuilder.Entity("SharedDiscord.User", b =>
                {
                    b.Property<ulong>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Discriminator")
                        .HasColumnType("char(4)");

                    b.Property<string>("Username");

                    b.HasKey("Id");

                    b.ToTable("user");
                });

            modelBuilder.Entity("SharedDiscord.GuildEmote", b =>
                {
                    b.HasOne("SharedDiscord.User", "User")
                        .WithMany("Emotes")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.SetNull);
                });

            modelBuilder.Entity("SharedDiscord.Message", b =>
                {
                    b.HasOne("SharedDiscord.User", "User")
                        .WithMany("Messages")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.SetNull);
                });

            modelBuilder.Entity("SharedDiscord.MessageEdit", b =>
                {
                    b.HasOne("SharedDiscord.Message", "Message")
                        .WithMany("MessageEdits")
                        .HasForeignKey("MessageId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
