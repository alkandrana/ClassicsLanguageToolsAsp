using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClassicsLanguageToolsAsp.Migrations
{
    /// <inheritdoc />
    public partial class UpdateComment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Instances_AspNetUsers_CreatorId",
                table: "Instances");

            migrationBuilder.DropIndex(
                name: "IX_Instances_CreatorId",
                table: "Instances");

            migrationBuilder.DropColumn(
                name: "CreatorId",
                table: "Instances");

            migrationBuilder.AddColumn<string>(
                name: "Reference",
                table: "Comments",
                type: "TEXT",
                maxLength: 255,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Reference",
                table: "Comments");

            migrationBuilder.AddColumn<string>(
                name: "CreatorId",
                table: "Instances",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Instances_CreatorId",
                table: "Instances",
                column: "CreatorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Instances_AspNetUsers_CreatorId",
                table: "Instances",
                column: "CreatorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
