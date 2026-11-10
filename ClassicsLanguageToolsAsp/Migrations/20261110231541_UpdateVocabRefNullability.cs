using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClassicsLanguageToolsAsp.Migrations
{
    /// <inheritdoc />
    public partial class UpdateVocabRefNullability : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Instances_Lines_LineId",
                table: "Instances");

            migrationBuilder.DropColumn(
                name: "Citation",
                table: "Instances");

            migrationBuilder.AlterColumn<int>(
                name: "LineId",
                table: "Instances",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Instances_Lines_LineId",
                table: "Instances",
                column: "LineId",
                principalTable: "Lines",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Instances_Lines_LineId",
                table: "Instances");

            migrationBuilder.AlterColumn<int>(
                name: "LineId",
                table: "Instances",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AddColumn<string>(
                name: "Citation",
                table: "Instances",
                type: "TEXT",
                maxLength: 255,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddForeignKey(
                name: "FK_Instances_Lines_LineId",
                table: "Instances",
                column: "LineId",
                principalTable: "Lines",
                principalColumn: "Id");
        }
    }
}
