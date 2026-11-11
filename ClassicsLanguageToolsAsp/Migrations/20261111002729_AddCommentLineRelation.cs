using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClassicsLanguageToolsAsp.Migrations
{
    /// <inheritdoc />
    public partial class AddCommentLineRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "LineId",
                table: "Comments",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Comments_LineId",
                table: "Comments",
                column: "LineId");

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_Lines_LineId",
                table: "Comments",
                column: "LineId",
                principalTable: "Lines",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comments_Lines_LineId",
                table: "Comments");

            migrationBuilder.DropIndex(
                name: "IX_Comments_LineId",
                table: "Comments");

            migrationBuilder.DropColumn(
                name: "LineId",
                table: "Comments");
        }
    }
}
