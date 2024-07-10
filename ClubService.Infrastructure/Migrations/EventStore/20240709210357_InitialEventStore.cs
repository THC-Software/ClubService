using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

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
                constraints: table =>
                {
                    table.PrimaryKey("pK_DomainEvent", x => x.eventId);
                });

            migrationBuilder.InsertData(
                table: "DomainEvent",
                columns: new[] { "eventId", "entityId", "entityType", "eventData", "eventType", "timestamp" },
                values: new object[,]
                {
                    { new Guid("36db98d7-8fea-4715-923c-74192b147752"), new Guid("4c148d45-ebc8-4bbf-aa9a-d491eb185ad5"), "SUBSCRIPTION_TIER", "{\"Id\":{\"Id\":\"4c148d45-ebc8-4bbf-aa9a-d491eb185ad5\"},\"Name\":\"Guinea Pig Subscription Tier\",\"MaxMemberCount\":100}", "SUBSCRIPTION_TIER_CREATED", new DateTime(2024, 7, 9, 21, 3, 56, 835, DateTimeKind.Utc).AddTicks(7913) },
                    { new Guid("3b591696-d9c9-4e30-a6a1-6a1439c5580b"), new Guid("2bebd11c-bf8e-4448-886f-0cb8608af7ca"), "SUBSCRIPTION_TIER", "{\"Id\":{\"Id\":\"2bebd11c-bf8e-4448-886f-0cb8608af7ca\"},\"Name\":\"Woolf Subscription Tier\",\"MaxMemberCount\":150}", "SUBSCRIPTION_TIER_CREATED", new DateTime(2024, 7, 9, 21, 3, 56, 835, DateTimeKind.Utc).AddTicks(7916) },
                    { new Guid("8d4d3eff-b77b-4e21-963b-e211366bb94b"), new Guid("38888969-d579-46ec-9cd6-0208569a077e"), "SUBSCRIPTION_TIER", "{\"Id\":{\"Id\":\"38888969-d579-46ec-9cd6-0208569a077e\"},\"Name\":\"Gorilla Subscription Tier\",\"MaxMemberCount\":200}", "SUBSCRIPTION_TIER_CREATED", new DateTime(2024, 7, 9, 21, 3, 56, 835, DateTimeKind.Utc).AddTicks(7918) },
                    { new Guid("ba4eeaa4-9707-4da0-8ee2-3684b4f7804f"), new Guid("1588ec27-c932-4dee-a341-d18c8108a711"), "SYSTEM_OPERATOR", "{\"SystemOperatorId\":{\"Id\":\"1588ec27-c932-4dee-a341-d18c8108a711\"},\"Username\":\"systemoperator\"}", "SYSTEM_OPERATOR_REGISTERED", new DateTime(2024, 7, 9, 21, 3, 56, 835, DateTimeKind.Utc).AddTicks(7907) },
                    { new Guid("e335d85a-f844-4c7e-b608-035ef00af733"), new Guid("d19073ba-f760-4a9a-abfa-f8215d96bec7"), "SUBSCRIPTION_TIER", "{\"Id\":{\"Id\":\"d19073ba-f760-4a9a-abfa-f8215d96bec7\"},\"Name\":\"Bison Subscription Tier\",\"MaxMemberCount\":250}", "SUBSCRIPTION_TIER_CREATED", new DateTime(2024, 7, 9, 21, 3, 56, 835, DateTimeKind.Utc).AddTicks(7921) }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DomainEvent");
        }
    }
}
