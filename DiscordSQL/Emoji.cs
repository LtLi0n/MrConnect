using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using LionLibrary.SQL;

namespace DiscordSQL
{
    [Table("emoji")]
    public class Emoji : IEntity<Emoji, string>
    {
        [Key] [Column("id")]
        public string Id { get; set; }

        [Required] [Column("name")]
        public string Name { get; set; }

        [Column("user?")]
        public string UserId { get; set; }
        public User User { get; set; }

        [Column("require_colons?")]
        public bool RequiresColons { get; set; } = false;

        [Column("managed?")]
        public bool IsManaged { get; set; } = false;

        [Column("animated?")]
        public bool IsAnimated { get; set; } = false;
    }
}
