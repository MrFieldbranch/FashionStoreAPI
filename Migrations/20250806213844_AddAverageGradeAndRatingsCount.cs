using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FashionStoreAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddAverageGradeAndRatingsCount : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "AverageGrade",
                table: "Products",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<int>(
                name: "RatingsCount",
                table: "Products",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AverageGrade",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "RatingsCount",
                table: "Products");
        }
    }
}
