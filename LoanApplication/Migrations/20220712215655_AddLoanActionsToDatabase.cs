using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LoanApplication.Migrations
{
    public partial class AddLoanActionsToDatabase : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LoanActions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LoanId = table.Column<int>(type: "int", nullable: false),
                    GiverUserId = table.Column<int>(type: "int", nullable: false),
                    TakerUserId = table.Column<int>(type: "int", nullable: false),
                    Purpose = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Amount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LoanActions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LoanActions_Loans_LoanId",
                        column: x => x.LoanId,
                        principalTable: "Loans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LoanActions_Users_GiverUserId",
                        column: x => x.GiverUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_LoanActions_Users_TakerUserId",
                        column: x => x.TakerUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_LoanActions_GiverUserId",
                table: "LoanActions",
                column: "GiverUserId");

            migrationBuilder.CreateIndex(
                name: "IX_LoanActions_LoanId",
                table: "LoanActions",
                column: "LoanId");

            migrationBuilder.CreateIndex(
                name: "IX_LoanActions_TakerUserId",
                table: "LoanActions",
                column: "TakerUserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LoanActions");
        }
    }
}
