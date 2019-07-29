using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using LionLibrary.SQL;
using System.Text;

namespace WarsOfTanothShared.SQL
{
    [Table("item")]
    public abstract class Item : IEntity<Item, uint>
    {
        [Key] [Column("id")]
        public uint Id { get; set; }

        [Required] [Column("name", TypeName = "varchar(255)")]
        public string Name { get; set; }

        [Required] [Column("code_name", TypeName = "varchar(255)")]
        public string CodeName { get; set; }

        [Column("emoji_id")]
        public uint EmojiId { get; }
        public Emoji Emoji { get; set; }

        [Column("description", TypeName = "varchar(255)")]
        public string Description { get; set; }
    }
}
