using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CareSync.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddBillTaxRate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "TaxRate",
                table: "Bills",
                type: "decimal(5,4)",
                nullable: false,
                defaultValue: 0.00m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TaxRate",
                table: "Bills");
        }
    }
}
