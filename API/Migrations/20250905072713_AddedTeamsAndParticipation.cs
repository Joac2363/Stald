using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Migrations
{
    /// <inheritdoc />
    public partial class AddedTeamsAndParticipation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EventParticipations",
                columns: table => new
                {
                    EventId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventParticipations", x => x.EventId);
                    table.ForeignKey(
                        name: "FK_EventParticipations_Events_EventId",
                        column: x => x.EventId,
                        principalTable: "Events",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EventParticipationTeam",
                columns: table => new
                {
                    EventParticipationsEventId = table.Column<Guid>(type: "uuid", nullable: false),
                    TeamsId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventParticipationTeam", x => new { x.EventParticipationsEventId, x.TeamsId });
                    table.ForeignKey(
                        name: "FK_EventParticipationTeam_EventParticipations_EventParticipati~",
                        column: x => x.EventParticipationsEventId,
                        principalTable: "EventParticipations",
                        principalColumn: "EventId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EventParticipationTeam_Teams_TeamsId",
                        column: x => x.TeamsId,
                        principalTable: "Teams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EventParticipationUser",
                columns: table => new
                {
                    EventParticipationsEventId = table.Column<Guid>(type: "uuid", nullable: false),
                    UsersId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventParticipationUser", x => new { x.EventParticipationsEventId, x.UsersId });
                    table.ForeignKey(
                        name: "FK_EventParticipationUser_AspNetUsers_UsersId",
                        column: x => x.UsersId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EventParticipationUser_EventParticipations_EventParticipati~",
                        column: x => x.EventParticipationsEventId,
                        principalTable: "EventParticipations",
                        principalColumn: "EventId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EventParticipationTeam_TeamsId",
                table: "EventParticipationTeam",
                column: "TeamsId");

            migrationBuilder.CreateIndex(
                name: "IX_EventParticipationUser_UsersId",
                table: "EventParticipationUser",
                column: "UsersId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EventParticipationTeam");

            migrationBuilder.DropTable(
                name: "EventParticipationUser");

            migrationBuilder.DropTable(
                name: "EventParticipations");
        }
    }
}
