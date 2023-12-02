﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CollectiveKnowledgePlatform.Data.Migrations
{
    public partial class MigratieUseriRoluri3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Category_AspNetUsers_UserId",
                table: "Category");

            migrationBuilder.DropForeignKey(
                name: "FK_Comment_AspNetUsers_UserId",
                table: "Comment");

            migrationBuilder.DropForeignKey(
                name: "FK_Topic_AspNetUsers_UserId",
                table: "Topic");

            migrationBuilder.DropForeignKey(
                name: "FK_TopicLike_AspNetUsers_UserId",
                table: "TopicLike");

            migrationBuilder.DropIndex(
                name: "IX_TopicLike_UserId",
                table: "TopicLike");

            migrationBuilder.DropIndex(
                name: "IX_Topic_UserId",
                table: "Topic");

            migrationBuilder.DropIndex(
                name: "IX_Comment_UserId",
                table: "Comment");

            migrationBuilder.DropIndex(
                name: "IX_Category_UserId",
                table: "Category");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "TopicLike",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<string>(
                name: "ApplicationUserId",
                table: "TopicLike",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "Topic",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<string>(
                name: "ApplicationUserId",
                table: "Topic",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "Comment",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<string>(
                name: "ApplicationUserId",
                table: "Comment",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "Category",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<string>(
                name: "ApplicationUserId",
                table: "Category",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TopicLike_ApplicationUserId",
                table: "TopicLike",
                column: "ApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Topic_ApplicationUserId",
                table: "Topic",
                column: "ApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Comment_ApplicationUserId",
                table: "Comment",
                column: "ApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Category_ApplicationUserId",
                table: "Category",
                column: "ApplicationUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Category_AspNetUsers_ApplicationUserId",
                table: "Category",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Comment_AspNetUsers_ApplicationUserId",
                table: "Comment",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Topic_AspNetUsers_ApplicationUserId",
                table: "Topic",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TopicLike_AspNetUsers_ApplicationUserId",
                table: "TopicLike",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Category_AspNetUsers_ApplicationUserId",
                table: "Category");

            migrationBuilder.DropForeignKey(
                name: "FK_Comment_AspNetUsers_ApplicationUserId",
                table: "Comment");

            migrationBuilder.DropForeignKey(
                name: "FK_Topic_AspNetUsers_ApplicationUserId",
                table: "Topic");

            migrationBuilder.DropForeignKey(
                name: "FK_TopicLike_AspNetUsers_ApplicationUserId",
                table: "TopicLike");

            migrationBuilder.DropIndex(
                name: "IX_TopicLike_ApplicationUserId",
                table: "TopicLike");

            migrationBuilder.DropIndex(
                name: "IX_Topic_ApplicationUserId",
                table: "Topic");

            migrationBuilder.DropIndex(
                name: "IX_Comment_ApplicationUserId",
                table: "Comment");

            migrationBuilder.DropIndex(
                name: "IX_Category_ApplicationUserId",
                table: "Category");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId",
                table: "TopicLike");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId",
                table: "Topic");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId",
                table: "Comment");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId",
                table: "Category");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "TopicLike",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "Topic",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "Comment",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "Category",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_TopicLike_UserId",
                table: "TopicLike",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Topic_UserId",
                table: "Topic",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Comment_UserId",
                table: "Comment",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Category_UserId",
                table: "Category",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Category_AspNetUsers_UserId",
                table: "Category",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Comment_AspNetUsers_UserId",
                table: "Comment",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Topic_AspNetUsers_UserId",
                table: "Topic",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TopicLike_AspNetUsers_UserId",
                table: "TopicLike",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
