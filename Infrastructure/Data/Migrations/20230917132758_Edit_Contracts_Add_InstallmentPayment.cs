using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Data.Migrations
{
    public partial class Edit_Contracts_Add_InstallmentPayment : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Funders_Name",
                table: "Funders");

            migrationBuilder.AddColumn<decimal>(
                name: "TotalFundingAmount",
                table: "Contracts",
                type: "decimal(17,2)",
                precision: 17,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalInstallmentsAmount",
                table: "Contracts",
                type: "decimal(17,2)",
                precision: 17,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateTable(
                name: "InstallmentPayments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InstallmentNumber = table.Column<int>(type: "int", nullable: false),
                    InstallmentAmount = table.Column<decimal>(type: "decimal(17,2)", precision: 17, scale: 2, nullable: false),
                    PaymentAmount = table.Column<decimal>(type: "decimal(17,2)", precision: 17, scale: 2, nullable: false),
                    OtherPaymentsAmount = table.Column<decimal>(type: "decimal(17,2)", precision: 17, scale: 2, nullable: false),
                    IsNet = table.Column<bool>(type: "bit", nullable: false),
                    IsBank = table.Column<bool>(type: "bit", nullable: false),
                    PaymentDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    BankRefNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TransferredBankAccountNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BankStatement = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ContractId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InstallmentPayments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InstallmentPayments_Contracts_ContractId",
                        column: x => x.ContractId,
                        principalTable: "Contracts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Funders_Name_MainNumber_SubNumber",
                table: "Funders",
                columns: new[] { "Name", "MainNumber", "SubNumber" },
                unique: true,
                filter: "[MainNumber] IS NOT NULL AND [SubNumber] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_InstallmentPayments_ContractId",
                table: "InstallmentPayments",
                column: "ContractId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "InstallmentPayments");

            migrationBuilder.DropIndex(
                name: "IX_Funders_Name_MainNumber_SubNumber",
                table: "Funders");

            migrationBuilder.DropColumn(
                name: "TotalFundingAmount",
                table: "Contracts");

            migrationBuilder.DropColumn(
                name: "TotalInstallmentsAmount",
                table: "Contracts");

            migrationBuilder.CreateIndex(
                name: "IX_Funders_Name",
                table: "Funders",
                column: "Name",
                unique: true);
        }
    }
}
