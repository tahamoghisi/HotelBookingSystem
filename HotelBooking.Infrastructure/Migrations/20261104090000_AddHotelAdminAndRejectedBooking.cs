using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HotelBooking.Infrastructure.Migrations
{
    public partial class AddHotelAdminAndRejectedBooking : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AdminUserId",
                table: "Hotels",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Hotels_AdminUserId",
                table: "Hotels",
                column: "AdminUserId",
                unique: true,
                filter: "[AdminUserId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Hotels_Users_AdminUserId",
                table: "Hotels",
                column: "AdminUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(name: "FK_Hotels_Users_AdminUserId", table: "Hotels");
            migrationBuilder.DropIndex(name: "IX_Hotels_AdminUserId", table: "Hotels");
            migrationBuilder.DropColumn(name: "AdminUserId", table: "Hotels");
        }
    }
}
