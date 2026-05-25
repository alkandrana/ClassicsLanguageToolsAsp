using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClassicsLanguageToolsAsp.Migrations
{
    /// <inheritdoc />
    public partial class InstanceProjectRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateOnly>(
                name: "Deadline",
                table: "Projects",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(DateOnly),
                oldType: "TEXT");

            migrationBuilder.AddColumn<int>(
                name: "ProjectId",
                table: "Instances",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Instances_ProjectId",
                table: "Instances",
                column: "ProjectId");

            migrationBuilder.AddForeignKey(
                name: "FK_Instances_Projects_ProjectId",
                table: "Instances",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Instances_Projects_ProjectId",
                table: "Instances");

            migrationBuilder.DropIndex(
                name: "IX_Instances_ProjectId",
                table: "Instances");

            migrationBuilder.DropColumn(
                name: "ProjectId",
                table: "Instances");

            migrationBuilder.AlterColumn<DateOnly>(
                name: "Deadline",
                table: "Projects",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1),
                oldClrType: typeof(DateOnly),
                oldType: "TEXT",
                oldNullable: true);
        }
    }
}
