using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FashionStoreAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddSumOfGradesToProduct : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SumOfGrades",
                table: "Products",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SumOfGrades",
                table: "Products");
        }
    }
}
