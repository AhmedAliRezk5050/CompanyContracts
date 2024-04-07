using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Data.Migrations
{
    public partial class Contracts_Add_TaxRatio : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "TaxRatio",
                table: "Contracts",
                type: "float",
                nullable: false,
                defaultValue: 0.15);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TaxRatio",
                table: "Contracts");
        }
    }
}
