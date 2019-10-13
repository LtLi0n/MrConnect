using Discord;

namespace MrConnect.Shared
{
    public class SharedDiscordEmbedAuthor
    {
        public string Name { get; set; }
        public string Url { get; set; }
        public string IconUrl { get; set; }
        
        public EmbedAuthorBuilder ToEmbedAuthorBuilder() =>
            new EmbedAuthorBuilder
            {
                Name = Name,
                Url = Url,
                IconUrl = IconUrl
            };
    }
}
