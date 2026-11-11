using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClassicsLanguageToolsAsp.Migrations
{
    /// <inheritdoc />
    public partial class AddLineVocabRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "LineId",
                table: "Instances",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Instances_LineId",
                table: "Instances",
                column: "LineId");

            migrationBuilder.AddForeignKey(
                name: "FK_Instances_Lines_LineId",
                table: "Instances",
                column: "LineId",
                principalTable: "Lines",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Instances_Lines_LineId",
                table: "Instances");

            migrationBuilder.DropIndex(
                name: "IX_Instances_LineId",
                table: "Instances");

            migrationBuilder.DropColumn(
                name: "LineId",
                table: "Instances");
        }
    }
}
