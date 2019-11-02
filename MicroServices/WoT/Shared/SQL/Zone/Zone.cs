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
    [Table("zones")]
    public class Zone : IEntity<Zone, uint>
    {
        public static class Ref
        {
            public const string CodeName = "CodeName";
            public const string Name = "Name";
            public const string Description = "Description";
        }

        [Key]
        public uint Id { get; set; }

        [Required]
        public string CodeName { get; set; } = string.Empty;
        
        [Required]
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        [JsonIgnore, IgnoreDataMember]
        public ICollection<ZoneNode> Nodes { get; set; }

        public static void CreateModel(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Zone>(x =>
            {
                x.HasIndex(zone => zone.CodeName).IsUnique();

                x.HasMany(zone => zone.Nodes)
                .WithOne(node => node.Zone)
                .HasForeignKey(node => node.ZoneId);
            });
        }
    }
}
