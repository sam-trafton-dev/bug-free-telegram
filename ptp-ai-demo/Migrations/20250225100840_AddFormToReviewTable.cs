using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ptp_ai_demo.Migrations
{
    /// <inheritdoc />
    public partial class AddFormToReviewTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SignedBy",
                table: "PreTaskPlanInputs",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "PreTaskPlanInputs",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "FormsToReviews",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PreTaskPlanId = table.Column<int>(type: "int", nullable: false),
                    SignedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UpdatedGeneratedJson = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SubmittedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FormsToReviews", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FormsToReviews");

            migrationBuilder.DropColumn(
                name: "SignedBy",
                table: "PreTaskPlanInputs");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "PreTaskPlanInputs");
        }
    }
}
