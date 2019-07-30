using Microsoft.EntityFrameworkCore.Migrations;

namespace ServerDiscord.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "user",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Username = table.Column<string>(nullable: true),
                    Discriminator = table.Column<string>(type: "char(4)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "guild_emote",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    GuildId = table.Column<string>(nullable: false),
                    UserId = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: false),
                    RequireColons = table.Column<bool>(nullable: false),
                    IsManaged = table.Column<bool>(nullable: false),
                    animated = table.Column<bool>(name: "animated?", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_guild_emote", x => x.Id);
                    table.ForeignKey(
                        name: "FK_guild_emote_user_UserId",
                        column: x => x.UserId,
                        principalTable: "user",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_guild_emote_UserId",
                table: "guild_emote",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "guild_emote");

            migrationBuilder.DropTable(
                name: "user");
        }
    }
}
