using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QRmenuAPI.Migrations
{
    public partial class RU_ForeignKey : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_RestaurantUsers",
                table: "RestaurantUsers");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "RestaurantUsers");

            migrationBuilder.AddColumn<string>(
                name: "UserApplicationUserId",
                table: "RestaurantUsers",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RestaurantUsers",
                table: "RestaurantUsers",
                columns: new[] { "UserApplicationUserId", "RestaurantId" });

            migrationBuilder.CreateIndex(
                name: "IX_RestaurantUsers_RestaurantId",
                table: "RestaurantUsers",
                column: "RestaurantId");

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
                onDelete: ReferentialAction.NoAction);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RestaurantUsers_AspNetUsers_UserApplicationUserId",
                table: "RestaurantUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_RestaurantUsers_Restaurants_RestaurantId",
                table: "RestaurantUsers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RestaurantUsers",
                table: "RestaurantUsers");

            migrationBuilder.DropIndex(
                name: "IX_RestaurantUsers_RestaurantId",
                table: "RestaurantUsers");

            migrationBuilder.DropColumn(
                name: "UserApplicationUserId",
                table: "RestaurantUsers");

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "RestaurantUsers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_RestaurantUsers",
                table: "RestaurantUsers",
                columns: new[] { "UserId", "RestaurantId" });
        }
    }
}
