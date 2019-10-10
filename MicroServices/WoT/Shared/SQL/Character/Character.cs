using DataServerHelpers;
using LionLibrary.SQL;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using System.Text;

namespace WoT.Shared
{
    [Table("characters")]
    public class Character : IEntity<Character, uint>
    {
        public static class Ref
        {
            public const string UserId = "UserId";
            public const string Name = "Name";
        }

        [Key]
        public uint Id { get; set; }

        [Required]
        public uint UserId { get; set; }
        [JsonIgnore, IgnoreDataMember]
        public User User { get; set; }

        [Required, Column(TypeName = "varchar(32)")]
        public string Name { get; set; }

        public Character_Skills Skills { get; set; }

        public CharacterWork Work { get; set; }

        public static void CreateModel(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Character>(x =>
            {
                x.HasIndex(x => x.Name).IsUnique();
                
                x.HasOne(character => character.Work)
                .WithOne(work => work.Character)
                .HasForeignKey<CharacterWork>(work => work.CharacterId);
            });

            CharacterWork.CreateModel(modelBuilder);
        }
    }
}
