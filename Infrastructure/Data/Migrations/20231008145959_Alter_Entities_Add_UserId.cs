using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Data.Migrations
{
    public partial class Alter_Entities_Add_UserId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "InstallmentPayments",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Funders",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Destructions",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Contracts",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_InstallmentPayments_UserId",
                table: "InstallmentPayments",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Funders_UserId",
                table: "Funders",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Destructions_UserId",
                table: "Destructions",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Contracts_UserId",
                table: "Contracts",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Contracts_AspNetUsers_UserId",
                table: "Contracts",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Destructions_AspNetUsers_UserId",
                table: "Destructions",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Funders_AspNetUsers_UserId",
                table: "Funders",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_InstallmentPayments_AspNetUsers_UserId",
                table: "InstallmentPayments",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Contracts_AspNetUsers_UserId",
                table: "Contracts");

            migrationBuilder.DropForeignKey(
                name: "FK_Destructions_AspNetUsers_UserId",
                table: "Destructions");

            migrationBuilder.DropForeignKey(
                name: "FK_Funders_AspNetUsers_UserId",
                table: "Funders");

            migrationBuilder.DropForeignKey(
                name: "FK_InstallmentPayments_AspNetUsers_UserId",
                table: "InstallmentPayments");

            migrationBuilder.DropIndex(
                name: "IX_InstallmentPayments_UserId",
                table: "InstallmentPayments");

            migrationBuilder.DropIndex(
                name: "IX_Funders_UserId",
                table: "Funders");

            migrationBuilder.DropIndex(
                name: "IX_Destructions_UserId",
                table: "Destructions");

            migrationBuilder.DropIndex(
                name: "IX_Contracts_UserId",
                table: "Contracts");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "InstallmentPayments");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Funders");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Destructions");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Contracts");
        }
    }
}
