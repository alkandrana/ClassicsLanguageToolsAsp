using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClassicsLanguageToolsAsp.Migrations
{
    /// <inheritdoc />
    public partial class OptionalCreator : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Vocab_AspNetUsers_CreatorId",
                table: "Vocab");

            migrationBuilder.AlterColumn<string>(
                name: "CreatorId",
                table: "Vocab",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AddForeignKey(
                name: "FK_Vocab_AspNetUsers_CreatorId",
                table: "Vocab",
                column: "CreatorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Vocab_AspNetUsers_CreatorId",
                table: "Vocab");

            migrationBuilder.AlterColumn<string>(
                name: "CreatorId",
                table: "Vocab",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Vocab_AspNetUsers_CreatorId",
                table: "Vocab",
                column: "CreatorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
