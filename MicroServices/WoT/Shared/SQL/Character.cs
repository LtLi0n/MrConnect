using LionLibrary.SQL;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using System.Text;

namespace WoT.Shared.SQL
{
    [Table("character")]
    public class Character : IEntity<Character, uint>
    {
        [Key, Column("id")]
        public uint Id { get; set; }

        [Required, Column("user_id")]
        public uint UserId { get; set; }
        [JsonIgnore, IgnoreDataMember]
        public User User { get; set; }

        [Required, Column("name", TypeName = "varchar(20)")]
        public string Name { get; set; }

        public Character_Skills Skills { get; set; }

        public static void CreateModel(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Character>(x =>
            {
                x.HasIndex(x => x.Name).IsUnique();
            });
        }
    }
}
