using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WoT.Server.Migrations
{
    public partial class CharacterGoldAndWorkerChanges : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StartedWorkAt",
                table: "characters_work");

            migrationBuilder.AddColumn<byte>(
                name: "CommittedHours",
                table: "characters_work",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<ulong>(
                name: "Gold",
                table: "characters",
                nullable: false,
                defaultValue: 0ul);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CommittedHours",
                table: "characters_work");

            migrationBuilder.DropColumn(
                name: "Gold",
                table: "characters");

            migrationBuilder.AddColumn<DateTime>(
                name: "StartedWorkAt",
                table: "characters_work",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}
