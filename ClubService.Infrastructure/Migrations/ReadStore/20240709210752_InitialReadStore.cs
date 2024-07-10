using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClubService.Infrastructure.Migrations.ReadStore
{
    /// <inheritdoc />
    public partial class InitialReadStore : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Admin",
                columns: table => new
                {
                    adminId = table.Column<Guid>(type: "uuid", nullable: false),
                    username = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    FirstName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    LastName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    tennisClubId = table.Column<Guid>(type: "uuid", nullable: false),
                    status = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pK_Admin", x => x.adminId);
                });

            migrationBuilder.CreateTable(
                name: "EmailOutbox",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    recipientEMailAddress = table.Column<string>(type: "text", nullable: false),
                    subject = table.Column<string>(type: "text", nullable: false),
                    body = table.Column<string>(type: "text", nullable: false),
                    timestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pK_EmailOutbox", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Member",
                columns: table => new
                {
                    memberId = table.Column<Guid>(type: "uuid", nullable: false),
                    FirstName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    LastName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    email = table.Column<string>(type: "text", nullable: false),
                    tennisClubId = table.Column<Guid>(type: "uuid", nullable: false),
                    status = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pK_Member", x => x.memberId);
                });

            migrationBuilder.CreateTable(
                name: "ProcessedEvent",
                columns: table => new
                {
                    eventId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pK_ProcessedEvent", x => x.eventId);
                });

            migrationBuilder.CreateTable(
                name: "SubscriptionTier",
                columns: table => new
                {
                    subscriptionTierId = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    maxMemberCount = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pK_SubscriptionTier", x => x.subscriptionTierId);
                });

            migrationBuilder.CreateTable(
                name: "SystemOperator",
                columns: table => new
                {
                    systemOperatorId = table.Column<Guid>(type: "uuid", nullable: false),
                    username = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pK_SystemOperator", x => x.systemOperatorId);
                });

            migrationBuilder.CreateTable(
                name: "TennisClub",
                columns: table => new
                {
                    tennisClubId = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    subscriptionTierId = table.Column<Guid>(type: "uuid", nullable: false),
                    status = table.Column<string>(type: "text", nullable: false),
                    memberCount = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pK_TennisClub", x => x.tennisClubId);
                });

            migrationBuilder.CreateTable(
                name: "Tournament",
                columns: table => new
                {
                    tournamentId = table.Column<Guid>(type: "uuid", nullable: false),
                    tennisClubId = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    startDate = table.Column<DateOnly>(type: "date", nullable: false),
                    endDate = table.Column<DateOnly>(type: "date", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pK_Tournament", x => x.tournamentId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Admin");

            migrationBuilder.DropTable(
                name: "EmailOutbox");

            migrationBuilder.DropTable(
                name: "Member");

            migrationBuilder.DropTable(
                name: "ProcessedEvent");

            migrationBuilder.DropTable(
                name: "SubscriptionTier");

            migrationBuilder.DropTable(
                name: "SystemOperator");

            migrationBuilder.DropTable(
                name: "TennisClub");

            migrationBuilder.DropTable(
                name: "Tournament");
        }
    }
}
