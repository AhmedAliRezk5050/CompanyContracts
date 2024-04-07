using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Data.Migrations
{
    public partial class Alter_Contracts_Edit_DeleteBehavior : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InstallmentPayments_Contracts_ContractId",
                table: "InstallmentPayments");

            migrationBuilder.AddForeignKey(
                name: "FK_InstallmentPayments_Contracts_ContractId",
                table: "InstallmentPayments",
                column: "ContractId",
                principalTable: "Contracts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InstallmentPayments_Contracts_ContractId",
                table: "InstallmentPayments");

            migrationBuilder.AddForeignKey(
                name: "FK_InstallmentPayments_Contracts_ContractId",
                table: "InstallmentPayments",
                column: "ContractId",
                principalTable: "Contracts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
