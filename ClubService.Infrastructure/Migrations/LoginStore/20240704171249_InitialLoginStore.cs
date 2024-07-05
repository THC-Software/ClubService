#nullable disable

using Microsoft.EntityFrameworkCore.Migrations;

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
                constraints: table => { table.PrimaryKey("pK_UserPassword", x => x.userId); });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserPassword");
        }
    }
}