using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using LionLibrary.SQL;

namespace SharedDiscord
{
    [Table("guild_emote")]
    public class GuildEmote : IEntity<GuildEmote, string>
    {
        [Key]
        public string Id { get; set; }

        [Required]
        public string GuildId { get; set; }

        public string UserId { get; set; }
        public User User { get; set; }

        [Required]
        public string Name { get; set; }

        public bool RequireColons { get; set; } = false;
        public bool IsManaged { get; set; } = false;

        [Column("animated?")]
        public bool IsAnimated { get; set; } = false;
    }
}
