using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FashionStoreAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddQuantityToShoppingBasketItem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Quantity",
                table: "ShoppingBasketItems",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Quantity",
                table: "ShoppingBasketItems");
        }
    }
}
