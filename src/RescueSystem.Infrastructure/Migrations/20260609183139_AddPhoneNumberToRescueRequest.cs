using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RescueSystem.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPhoneNumberToRescueRequest : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PhoneNumber",
                table: "Requests",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PhoneNumber",
                table: "Requests");
        }
    }
}
