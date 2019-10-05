using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using LionLibrary.SQL;

namespace WoT.Shared
{
    [Table("items")]
    public abstract class Item : IEntity<Item, uint>
    {
        [Key, Column("id")]
        public uint Id { get; set; }

        [Required, Column("name", TypeName = "varchar(255)")]
        public string Name { get; set; }

        [Required, Column("code_name", TypeName = "varchar(255)")]
        public string CodeName { get; set; }

        [Column("emoji_id")]
        public uint EmoteId { get; }

        [Column("description", TypeName = "varchar(255)")]
        public string Description { get; set; }
    }
}
