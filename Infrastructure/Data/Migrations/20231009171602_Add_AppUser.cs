using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Data.Migrations
{
    public partial class Add_AppUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.AddForeignKey(
                name: "FK_Contracts_AspNetUsers_UserId",
                table: "Contracts",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Destructions_AspNetUsers_UserId",
                table: "Destructions",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Funders_AspNetUsers_UserId",
                table: "Funders",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_InstallmentPayments_AspNetUsers_UserId",
                table: "InstallmentPayments",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
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
    }
}
