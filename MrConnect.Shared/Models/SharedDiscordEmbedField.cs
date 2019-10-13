using Discord;

namespace MrConnect.Shared
{
    public class SharedDiscordEmbedField
    {
        public string Name { get; set; }
        public object Value { get; set; }
        public bool IsInline { get; set; }

        public EmbedFieldBuilder ToEmbedFieldBuilder() =>
            new EmbedFieldBuilder
            {
                Name = Name,
                Value = Value,
                IsInline = IsInline
            };
    }
}
