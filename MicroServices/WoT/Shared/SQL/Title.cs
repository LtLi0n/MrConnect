using LionLibrary.SQL;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace WoT.Shared
{
    [Table("titles")]
    public class Title : IEntity<Title, uint>
    {
        [Key, Column("id")]
        public uint Id { get; set; }

        [Column("name", TypeName = "varchar(255)")]
        public string Name { get; set; }

        [Column("reason", TypeName = "varchar(255)")]
        public string Reason { get; set; }

        [Column("path", TypeName = "varchar(255)")]
        public string Path { get; set; }

        public TitleCategoryType Category { get; set; }

        public uint Color =>
            Category switch
            {
                TitleCategoryType.Quest => 0x00_66_00,
                TitleCategoryType.MosterKills => 0x66_00_00,
                TitleCategoryType.Special => 0xAA_AA_00,
                TitleCategoryType.Leaderboard => 0x77_00_FF,
                TitleCategoryType.Event => 0x66_CC_AA,
                _ => 0x000000
            };

        public string CategoryName =>
            Category switch
            {
                TitleCategoryType.Quest => "Quest",
                TitleCategoryType.MosterKills => "MosterKills",
                TitleCategoryType.Special => "Special",
                TitleCategoryType.Leaderboard => "Leaderboard",
                TitleCategoryType.Event => "Event",
                _ => "Unknown, inform about this."
            };
    }
}
