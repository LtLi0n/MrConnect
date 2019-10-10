using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WoT.Server.Migrations
{
    public partial class CharacterWorkInitial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "characters",
                type: "varchar(32)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(20)");

            migrationBuilder.CreateTable(
                name: "characters_work",
                columns: table => new
                {
                    CharacterId = table.Column<uint>(nullable: false),
                    IsWorking = table.Column<bool>(nullable: false),
                    StartedWorkAt = table.Column<DateTime>(nullable: false),
                    WorkFinishesAt = table.Column<DateTime>(nullable: false),
                    TotalHours = table.Column<uint>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_characters_work", x => x.CharacterId);
                    table.ForeignKey(
                        name: "FK_characters_work_characters_CharacterId",
                        column: x => x.CharacterId,
                        principalTable: "characters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "characters_work");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "characters",
                type: "varchar(20)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(32)");
        }
    }
}
