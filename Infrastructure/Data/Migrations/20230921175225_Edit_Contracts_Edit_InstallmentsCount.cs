using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Data.Migrations
{
    public partial class Edit_Contracts_Edit_InstallmentsCount : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "InstallmentsCount",
                table: "Contracts",
                type: "int",
                precision: 17,
                scale: 2,
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float(17)",
                oldPrecision: 17,
                oldScale: 2);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<double>(
                name: "InstallmentsCount",
                table: "Contracts",
                type: "float(17)",
                precision: 17,
                scale: 2,
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldPrecision: 17,
                oldScale: 2);
        }
    }
}
