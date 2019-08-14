using LionLibrary.SQL;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SharedDiscord
{
    [Table("emoji")]
    public class GuildEmoji : IEntity<GuildEmoji, ulong>
    {
        public static class Ref
        {
            public const string id = "id";
            public const string guild_id = "guild_id";
            public const string name = "name";
            public const string animated = "animated?";
        }

        [Key, Column(Ref.id)]
        public ulong Id { get; set; }

        [Required, Column(Ref.guild_id)]
        public ulong GuildId { get; set; }

        [Required, Column(Ref.name, TypeName = "nvarchar(255)")]
        public string Name { get; set; }

        [Column(Ref.animated)]
        public bool IsAnimated { get; set; } = false;

        public override string ToString() => string.Format("<{0}:{1}:{2}>", IsAnimated ? "a" : string.Empty, Name, Id);
    }
}
