using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QRmenuAPI.Migrations
{
    public partial class RestaurantUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RestaurantUsers",
                columns: table => new
                {
                    RestaurantId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RestaurantUsers", x => new { x.UserId, x.RestaurantId });
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RestaurantUsers");
        }
    }
}
