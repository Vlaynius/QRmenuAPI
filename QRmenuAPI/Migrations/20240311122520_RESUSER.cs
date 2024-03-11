using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QRmenuAPI.Migrations
{
    public partial class RESUSER : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RestaurantUsers_AspNetUsers_UserApplicationUserId",
                table: "RestaurantUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_RestaurantUsers_Restaurants_RestaurantId",
                table: "RestaurantUsers");

            migrationBuilder.AddForeignKey(
                name: "FK_RestaurantUsers_AspNetUsers_UserApplicationUserId",
                table: "RestaurantUsers",
                column: "UserApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_RestaurantUsers_Restaurants_RestaurantId",
                table: "RestaurantUsers",
                column: "RestaurantId",
                principalTable: "Restaurants",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RestaurantUsers_AspNetUsers_UserApplicationUserId",
                table: "RestaurantUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_RestaurantUsers_Restaurants_RestaurantId",
                table: "RestaurantUsers");

            migrationBuilder.AddForeignKey(
                name: "FK_RestaurantUsers_AspNetUsers_UserApplicationUserId",
                table: "RestaurantUsers",
                column: "UserApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RestaurantUsers_Restaurants_RestaurantId",
                table: "RestaurantUsers",
                column: "RestaurantId",
                principalTable: "Restaurants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
