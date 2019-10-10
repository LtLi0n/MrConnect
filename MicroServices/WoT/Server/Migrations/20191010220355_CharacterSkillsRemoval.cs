using Microsoft.EntityFrameworkCore.Migrations;

namespace WoT.Server.Migrations
{
    public partial class CharacterSkillsRemoval : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "mining_xp",
                table: "characters");

            migrationBuilder.DropColumn(
                name: "woodcutting_xp",
                table: "characters");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<ulong>(
                name: "mining_xp",
                table: "characters",
                nullable: false,
                defaultValue: 0ul);

            migrationBuilder.AddColumn<ulong>(
                name: "woodcutting_xp",
                table: "characters",
                nullable: false,
                defaultValue: 0ul);
        }
    }
}
