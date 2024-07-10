using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ClubService.Infrastructure.Migrations.LoginStore
{
    /// <inheritdoc />
    public partial class InitialLoginStore : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserPassword",
                columns: table => new
                {
                    userId = table.Column<Guid>(type: "uuid", nullable: false),
                    hashedPassword = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pK_UserPassword", x => x.userId);
                });

            migrationBuilder.InsertData(
                table: "UserPassword",
                columns: new[] { "userId", "hashedPassword" },
                values: new object[,]
                {
                    { new Guid("1588ec27-c932-4dee-a341-d18c8108a711"), "AQAAAAIAAYagAAAAEDUKJ7ReV2uEyekYGEB91VvXGmWH+HwRO9wfUHnqgb5QHs0NJTac+/OUJbiInt64zg==" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserPassword");
        }
    }
}
