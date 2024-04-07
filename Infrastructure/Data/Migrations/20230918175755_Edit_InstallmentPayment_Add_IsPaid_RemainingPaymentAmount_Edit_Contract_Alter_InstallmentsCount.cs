using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Data.Migrations
{
    public partial class Edit_InstallmentPayment_Add_IsPaid_RemainingPaymentAmount_Edit_Contract_Alter_InstallmentsCount : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsPaid",
                table: "InstallmentPayments",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<decimal>(
                name: "RemainingPaymentAmount",
                table: "InstallmentPayments",
                type: "decimal(17,2)",
                precision: 17,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AlterColumn<double>(
                name: "InstallmentsCount",
                table: "Contracts",
                type: "float(17)",
                precision: 17,
                scale: 2,
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsPaid",
                table: "InstallmentPayments");

            migrationBuilder.DropColumn(
                name: "RemainingPaymentAmount",
                table: "InstallmentPayments");

            migrationBuilder.AlterColumn<int>(
                name: "InstallmentsCount",
                table: "Contracts",
                type: "int",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float(17)",
                oldPrecision: 17,
                oldScale: 2);
        }
    }
}
