using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClassicsLanguageToolsAsp.Migrations
{
    /// <inheritdoc />
    public partial class AddUserName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Instances_Projects_ProjectId",
                table: "Instances");

            migrationBuilder.AlterColumn<int>(
                name: "ProjectId",
                table: "Instances",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                table: "AspNetUsers",
                type: "TEXT",
                maxLength: 13,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "AspNetUsers",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Vocab_Lemma_CreatorId",
                table: "Vocab",
                columns: new[] { "Lemma", "CreatorId" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Instances_Projects_ProjectId",
                table: "Instances",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Instances_Projects_ProjectId",
                table: "Instances");

            migrationBuilder.DropIndex(
                name: "IX_Vocab_Lemma_CreatorId",
                table: "Vocab");

            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "AspNetUsers");

            migrationBuilder.AlterColumn<int>(
                name: "ProjectId",
                table: "Instances",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Instances_Projects_ProjectId",
                table: "Instances",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
