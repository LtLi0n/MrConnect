using LionLibrary.SQL;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace WoT.Shared
{
    [Table("titles_categories")]
    public class TitleCategory : IEntity<TitleCategory, uint>
    {
        [Key, Column("id")]
        public uint Id { get; set; }

        [Column("name", TypeName = "varchar(255)")]
        public string Name { get; set; }

        public uint Color { get; set; }
    }
}
