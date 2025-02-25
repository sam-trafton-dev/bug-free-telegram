using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ptp_ai_demo.Migrations
{
    /// <inheritdoc />
    public partial class typeFix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_FormsToReviews",
                table: "FormsToReviews");

            migrationBuilder.RenameTable(
                name: "FormsToReviews",
                newName: "FormsToReview");

            migrationBuilder.AddPrimaryKey(
                name: "PK_FormsToReview",
                table: "FormsToReview",
                column: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_FormsToReview",
                table: "FormsToReview");

            migrationBuilder.RenameTable(
                name: "FormsToReview",
                newName: "FormsToReviews");

            migrationBuilder.AddPrimaryKey(
                name: "PK_FormsToReviews",
                table: "FormsToReviews",
                column: "Id");
        }
    }
}
