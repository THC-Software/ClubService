#nullable disable

using Microsoft.EntityFrameworkCore.Migrations;

namespace ClubService.Infrastructure.Migrations.EventStore
{
    /// <inheritdoc />
    public partial class InitialEventStore : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DomainEvent",
                columns: table => new
                {
                    eventId = table.Column<Guid>(type: "uuid", nullable: false),
                    entityId = table.Column<Guid>(type: "uuid", nullable: false),
                    eventType = table.Column<string>(type: "text", nullable: false),
                    entityType = table.Column<string>(type: "text", nullable: false),
                    timestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    eventData = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table => { table.PrimaryKey("pK_DomainEvent", x => x.eventId); });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DomainEvent");
        }
    }
}