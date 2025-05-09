using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FashionStoreAPI.Migrations
{
    /// <inheritdoc />
    public partial class EnforceStockToBePositive : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("ALTER TABLE \"ProductVariants\" ADD CONSTRAINT \"CK_Stock_NonNegative\" CHECK (\"Stock\" >= 0);");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("ALTER TABLE \"ProductVariants\" DROP CONSTRAINT \"CK_Stock_NonNegative\";");
        }
    }
}
