﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CollectiveKnowledgePlatform.Data.Migrations
{
    public partial class MigratieUseriRoluri5 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Topic_Category_CategoryId",
                table: "Topic");

            migrationBuilder.AlterColumn<int>(
                name: "CategoryId",
                table: "Topic",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Topic_Category_CategoryId",
                table: "Topic",
                column: "CategoryId",
                principalTable: "Category",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Topic_Category_CategoryId",
                table: "Topic");

            migrationBuilder.AlterColumn<int>(
                name: "CategoryId",
                table: "Topic",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Topic_Category_CategoryId",
                table: "Topic",
                column: "CategoryId",
                principalTable: "Category",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
