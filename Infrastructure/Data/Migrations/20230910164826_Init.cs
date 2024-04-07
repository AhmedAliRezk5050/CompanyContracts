using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Data.Migrations
{
    public partial class Init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Funders",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ContactEmployee = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MainNumber = table.Column<int>(type: "int", nullable: false),
                    SubNumber = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Funders", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Contracts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ContractNumber = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    BasicFundingAmount = table.Column<decimal>(type: "decimal(17,2)", precision: 17, scale: 2, nullable: false),
                    InterestRatio = table.Column<double>(type: "float", nullable: false),
                    AdministrativeFees = table.Column<decimal>(type: "decimal(17,2)", precision: 17, scale: 2, nullable: false),
                    AdvancePayment = table.Column<decimal>(type: "decimal(17,2)", precision: 17, scale: 2, nullable: false),
                    InstallmentsCount = table.Column<int>(type: "int", nullable: false),
                    FirstInstallmentDate = table.Column<int>(type: "int", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FunderId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Contracts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Contracts_Funders_FunderId",
                        column: x => x.FunderId,
                        principalTable: "Funders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Contracts_ContractNumber",
                table: "Contracts",
                column: "ContractNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Contracts_FunderId",
                table: "Contracts",
                column: "FunderId");

            migrationBuilder.CreateIndex(
                name: "IX_Funders_Name",
                table: "Funders",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Funders_PhoneNumber",
                table: "Funders",
                column: "PhoneNumber",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Contracts");

            migrationBuilder.DropTable(
                name: "Funders");
        }
    }
}
