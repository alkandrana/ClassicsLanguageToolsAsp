using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClassicsLanguageToolsAsp.Migrations
{
    /// <inheritdoc />
    public partial class AddProjectLastLine : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Work",
                table: "Projects");

            migrationBuilder.AddColumn<int>(
                name: "LastLineId",
                table: "Projects",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "OpusId",
                table: "Projects",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ProjectId",
                table: "Comments",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Projects_LastLineId",
                table: "Projects",
                column: "LastLineId");

            migrationBuilder.CreateIndex(
                name: "IX_Projects_OpusId",
                table: "Projects",
                column: "OpusId");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_ProjectId",
                table: "Comments",
                column: "ProjectId");

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_Projects_ProjectId",
                table: "Comments",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Projects_Lines_LastLineId",
                table: "Projects",
                column: "LastLineId",
                principalTable: "Lines",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Projects_Opera_OpusId",
                table: "Projects",
                column: "OpusId",
                principalTable: "Opera",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comments_Projects_ProjectId",
                table: "Comments");

            migrationBuilder.DropForeignKey(
                name: "FK_Projects_Lines_LastLineId",
                table: "Projects");

            migrationBuilder.DropForeignKey(
                name: "FK_Projects_Opera_OpusId",
                table: "Projects");

            migrationBuilder.DropIndex(
                name: "IX_Projects_LastLineId",
                table: "Projects");

            migrationBuilder.DropIndex(
                name: "IX_Projects_OpusId",
                table: "Projects");

            migrationBuilder.DropIndex(
                name: "IX_Comments_ProjectId",
                table: "Comments");

            migrationBuilder.DropColumn(
                name: "LastLineId",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "OpusId",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "ProjectId",
                table: "Comments");

            migrationBuilder.AddColumn<string>(
                name: "Work",
                table: "Projects",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }
    }
}
