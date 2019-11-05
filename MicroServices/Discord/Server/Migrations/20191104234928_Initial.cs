using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Discord.Server.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    Id = table.Column<ulong>(nullable: false),
                    Username = table.Column<string>(type: "nvarchar(255)", nullable: false),
                    Discriminator = table.Column<string>(type: "varchar(4)", nullable: false),
                    LastUpdatedAt = table.Column<DateTime>(nullable: false),
                    AddedAt = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "facts",
                columns: table => new
                {
                    Id = table.Column<uint>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    UserId = table.Column<ulong>(nullable: false),
                    Content = table.Column<string>(nullable: false),
                    AddedAt = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_facts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_facts_users_UserId",
                        column: x => x.UserId,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "facts_suggestions",
                columns: table => new
                {
                    Id = table.Column<uint>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    UserId = table.Column<ulong>(nullable: false),
                    Content = table.Column<string>(nullable: false),
                    LastUpdatedAt = table.Column<DateTime>(nullable: false),
                    AddedAt = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_facts_suggestions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_facts_suggestions_users_UserId",
                        column: x => x.UserId,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "guilds",
                columns: table => new
                {
                    Id = table.Column<ulong>(nullable: false),
                    OwnerId = table.Column<ulong>(nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", nullable: false),
                    Prefix = table.Column<string>(type: "nvarchar(16)", nullable: true),
                    LastUpdatedAt = table.Column<DateTime>(nullable: false),
                    AddedAt = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_guilds", x => x.Id);
                    table.ForeignKey(
                        name: "FK_guilds_users_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_facts_UserId",
                table: "facts",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_facts_suggestions_UserId",
                table: "facts_suggestions",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_guilds_OwnerId",
                table: "guilds",
                column: "OwnerId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "facts");

            migrationBuilder.DropTable(
                name: "facts_suggestions");

            migrationBuilder.DropTable(
                name: "guilds");

            migrationBuilder.DropTable(
                name: "users");
        }
    }
}
