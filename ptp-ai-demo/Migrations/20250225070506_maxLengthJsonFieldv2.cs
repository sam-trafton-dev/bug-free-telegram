using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ptp_ai_demo.Migrations
{
    /// <inheritdoc />
    public partial class maxLengthJsonFieldv2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "GeneratedPtpJson",
                table: "PreTaskPlanInputs",
                type: "nvarchar(MAX)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "GeneratedPtpJson",
                table: "PreTaskPlanInputs",
                type: "nvarchar",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(MAX)",
                oldNullable: true);
        }
    }
}
