using Microsoft.EntityFrameworkCore.Migrations;

namespace Sky.Migrations.SkyAppDb
{
    public partial class InitialEdit3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OrderProductName",
                table: "CartDetail");

            migrationBuilder.AddColumn<bool>(
                name: "OrderLock",
                table: "Order",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "OrderTextId",
                table: "Order",
                nullable: false);

            migrationBuilder.AddColumn<int>(
                name: "ProductQuantity",
                table: "CartDetail",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OrderLock",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "OrderTextId",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "ProductQuantity",
                table: "CartDetail");

            migrationBuilder.AddColumn<string>(
                name: "OrderProductName",
                table: "CartDetail",
                type: "longtext CHARACTER SET utf8mb4",
                nullable: false,
                defaultValue: "");
        }
    }
}
