﻿using LionLibrary.SQL;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using System.Text;

namespace Discord.Shared
{
    [Table("users")]
    public class User : IDiscordEntity<User, ulong>
    {
        public static class Ref
        {
            public const string DiscordId = "DiscordId";
            public const string Username = "Username";
            public const string Discriminator = "Discriminator";
        }

        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public ulong Id { get; set; }

        [NotMapped]
        public ulong DiscordId { get => Id; set => Id = value; }

        [Column(TypeName = "nvarchar(255)")]
        public string Username { get; set; }

        [Column(TypeName = "varchar(4)")]
        public string Discriminator { get; set; }

        public DateTime LastUpdatedAt { get; set; }
        public DateTime AddedAt { get; set; } = DateTime.Now;


        ///<summary>Vsauce - You want some spit facts?</summary>
        [JsonIgnore, IgnoreDataMember]
        public ICollection<Fact> Facts { get; set; }

        [JsonIgnore, IgnoreDataMember]
        public ICollection<FactSuggestion> FactSuggestions { get; set; }

        public static void CreateModel(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(x =>
            {
                x.HasMany(user => user.Facts)
                .WithOne(fact => fact.User)
                .HasForeignKey(fact => fact.UserId);

                x.HasMany(user => user.FactSuggestions)
                .WithOne(factSug => factSug.User)
                .HasForeignKey(factSug => factSug.UserId);
            });
        }
    }
}