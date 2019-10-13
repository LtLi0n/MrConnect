using Discord;
using System.Collections.Generic;
using System.Linq;

namespace MrConnect.Shared
{
    public class SharedDiscordEmbed
    {
        public SharedDiscordEmbedAuthor Author { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public uint Color { get; set; }
        public List<SharedDiscordEmbedField> Fields { get; set; } = new List<SharedDiscordEmbedField>();

        public EmbedBuilder ToEmbedBuilder() =>
            new EmbedBuilder
            {
                Author = Author?.ToEmbedAuthorBuilder(),
                Title = Title,
                Description = Description,
                Color = new Color(Color),
                Fields = new List<EmbedFieldBuilder>(Fields.Select(x => x.ToEmbedFieldBuilder()))
            };
    }
}
