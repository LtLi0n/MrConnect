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
    [Table("zones_nodes")]
    public class ZoneNode : IEntity<ZoneNode, uint>
    {
        public static class Ref
        {
            public const string ZoneId = "ZoneId";

            public const string Name = "Name";
            public const string Description = "Description";

            public const string ZoneNodeNorthId = "ZoneNodeNorthId";
            public const string ZoneNodeSouthId = "ZoneNodeSouthId";
            public const string ZoneNodeEastId = "ZoneNodeEastId";
            public const string ZoneNodeWestId = "ZoneNodeWestId";
        }

        public uint Id { get; set; }

        [Required]
        public uint ZoneId { get; set; }
        public Zone Zone { get; set; }

        public string Name { get; set; }
        public string? Description { get; set; }

        public uint? ZoneNodeNorthId { get; set; }        
        public ZoneNode? ZoneNodeNorth { get; set; }

        public uint? ZoneNodeSouthId { get; set; }
        public ZoneNode? ZoneNodeSouth { get; set; }


        public uint? ZoneNodeEastId { get; set; }
        public ZoneNode? ZoneNodeEast { get; set; }

        public uint? ZoneNodeWestId { get; set; }
        public ZoneNode? ZoneNodeWest { get; set; }

        [JsonIgnore, IgnoreDataMember]
        public ICollection<Character> Characters { get; set; }

        public static void CreateModel(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ZoneNode>(x =>
            {
                x.HasMany(node => node.Characters)
                .WithOne(character => character.ZoneNode)
                .HasForeignKey(character => character.ZoneNodeId);

                x.HasOne(node => node.ZoneNodeNorth)
                .WithOne(linked_node => linked_node.ZoneNodeSouth)
                .HasForeignKey<ZoneNode>(linked_node => linked_node.ZoneNodeSouthId);

                x.HasOne(node => node.ZoneNodeSouth)
                .WithOne(linked_node => linked_node.ZoneNodeNorth)
                .HasForeignKey<ZoneNode>(linked_node => linked_node.ZoneNodeNorthId);

                x.HasOne(node => node.ZoneNodeEast)
                .WithOne(linked_node => linked_node.ZoneNodeWest)
                .HasForeignKey<ZoneNode>(linked_node => linked_node.ZoneNodeEastId);

                x.HasOne(node => node.ZoneNodeWest)
                .WithOne(linked_node => linked_node.ZoneNodeEast)
                .HasForeignKey<ZoneNode>(linked_node => linked_node.ZoneNodeWestId);
            });
        }
    }
}
