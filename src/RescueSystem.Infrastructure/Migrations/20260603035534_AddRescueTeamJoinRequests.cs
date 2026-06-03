using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RescueSystem.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddRescueTeamJoinRequests : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RescueTeamJoinRequests",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RescuerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RescueTeamId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RescueTeamJoinRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RescueTeamJoinRequests_RescueTeams_RescueTeamId",
                        column: x => x.RescueTeamId,
                        principalTable: "RescueTeams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RescueTeamJoinRequests_Users_RescuerId",
                        column: x => x.RescuerId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RescueTeamJoinRequests_RescuerId",
                table: "RescueTeamJoinRequests",
                column: "RescuerId");

            migrationBuilder.CreateIndex(
                name: "IX_RescueTeamJoinRequests_RescueTeamId",
                table: "RescueTeamJoinRequests",
                column: "RescueTeamId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RescueTeamJoinRequests");
        }
    }
}
