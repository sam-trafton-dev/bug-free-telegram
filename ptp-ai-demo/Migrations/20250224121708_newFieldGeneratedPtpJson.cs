using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ptp_ai_demo.Migrations
{
    /// <inheritdoc />
    public partial class newFieldGeneratedPtpJson : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "GeneratedPtpJson",
                table: "PreTaskPlanInputs",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GeneratedPtpJson",
                table: "PreTaskPlanInputs");
        }
    }
}
