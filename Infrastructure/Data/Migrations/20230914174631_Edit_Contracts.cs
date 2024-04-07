using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Data.Migrations
{
    public partial class Edit_Contracts : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FirstInstallmentDate",
                table: "Contracts");

            migrationBuilder.AddColumn<DateTime>(
                name: "FirstInstallmentBeginningDate",
                table: "Contracts",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FirstInstallmentBeginningDate",
                table: "Contracts");

            migrationBuilder.AddColumn<int>(
                name: "FirstInstallmentDate",
                table: "Contracts",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
