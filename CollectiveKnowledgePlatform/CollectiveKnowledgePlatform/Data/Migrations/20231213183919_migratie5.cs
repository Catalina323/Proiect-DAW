using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CollectiveKnowledgePlatform.Data.Migrations
{
    public partial class migratie5 : Migration
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
                name: "FK_Comment_Topic_TopicId",
                table: "Comment");

            migrationBuilder.DropForeignKey(
                name: "FK_Topic_AspNetUsers_UserId",
                table: "Topic");

            migrationBuilder.DropForeignKey(
                name: "FK_Topic_Category_CategoryId",
                table: "Topic");

            migrationBuilder.DropForeignKey(
                name: "FK_TopicLike_AspNetUsers_UserId",
                table: "TopicLike");

            migrationBuilder.DropForeignKey(
                name: "FK_TopicLike_Topic_TopicId",
                table: "TopicLike");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TopicLike",
                table: "TopicLike");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Topic",
                table: "Topic");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Comment",
                table: "Comment");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Category",
                table: "Category");

            migrationBuilder.RenameTable(
                name: "TopicLike",
                newName: "TopicLikes");

            migrationBuilder.RenameTable(
                name: "Topic",
                newName: "Topics");

            migrationBuilder.RenameTable(
                name: "Comment",
                newName: "Comments");

            migrationBuilder.RenameTable(
                name: "Category",
                newName: "Categories");

            migrationBuilder.RenameIndex(
                name: "IX_TopicLike_UserId",
                table: "TopicLikes",
                newName: "IX_TopicLikes_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_TopicLike_TopicId",
                table: "TopicLikes",
                newName: "IX_TopicLikes_TopicId");

            migrationBuilder.RenameIndex(
                name: "IX_Topic_UserId",
                table: "Topics",
                newName: "IX_Topics_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Topic_CategoryId",
                table: "Topics",
                newName: "IX_Topics_CategoryId");

            migrationBuilder.RenameIndex(
                name: "IX_Comment_UserId",
                table: "Comments",
                newName: "IX_Comments_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Comment_TopicId",
                table: "Comments",
                newName: "IX_Comments_TopicId");

            migrationBuilder.RenameIndex(
                name: "IX_Category_UserId",
                table: "Categories",
                newName: "IX_Categories_UserId");

            migrationBuilder.AlterColumn<int>(
                name: "CategoryId",
                table: "Topics",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TopicLikes",
                table: "TopicLikes",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Topics",
                table: "Topics",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Comments",
                table: "Comments",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Categories",
                table: "Categories",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Categories_AspNetUsers_UserId",
                table: "Categories",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_AspNetUsers_UserId",
                table: "Comments",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_Topics_TopicId",
                table: "Comments",
                column: "TopicId",
                principalTable: "Topics",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TopicLikes_AspNetUsers_UserId",
                table: "TopicLikes",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TopicLikes_Topics_TopicId",
                table: "TopicLikes",
                column: "TopicId",
                principalTable: "Topics",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Topics_AspNetUsers_UserId",
                table: "Topics",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Topics_Categories_CategoryId",
                table: "Topics",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Categories_AspNetUsers_UserId",
                table: "Categories");

            migrationBuilder.DropForeignKey(
                name: "FK_Comments_AspNetUsers_UserId",
                table: "Comments");

            migrationBuilder.DropForeignKey(
                name: "FK_Comments_Topics_TopicId",
                table: "Comments");

            migrationBuilder.DropForeignKey(
                name: "FK_TopicLikes_AspNetUsers_UserId",
                table: "TopicLikes");

            migrationBuilder.DropForeignKey(
                name: "FK_TopicLikes_Topics_TopicId",
                table: "TopicLikes");

            migrationBuilder.DropForeignKey(
                name: "FK_Topics_AspNetUsers_UserId",
                table: "Topics");

            migrationBuilder.DropForeignKey(
                name: "FK_Topics_Categories_CategoryId",
                table: "Topics");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Topics",
                table: "Topics");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TopicLikes",
                table: "TopicLikes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Comments",
                table: "Comments");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Categories",
                table: "Categories");

            migrationBuilder.RenameTable(
                name: "Topics",
                newName: "Topic");

            migrationBuilder.RenameTable(
                name: "TopicLikes",
                newName: "TopicLike");

            migrationBuilder.RenameTable(
                name: "Comments",
                newName: "Comment");

            migrationBuilder.RenameTable(
                name: "Categories",
                newName: "Category");

            migrationBuilder.RenameIndex(
                name: "IX_Topics_UserId",
                table: "Topic",
                newName: "IX_Topic_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Topics_CategoryId",
                table: "Topic",
                newName: "IX_Topic_CategoryId");

            migrationBuilder.RenameIndex(
                name: "IX_TopicLikes_UserId",
                table: "TopicLike",
                newName: "IX_TopicLike_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_TopicLikes_TopicId",
                table: "TopicLike",
                newName: "IX_TopicLike_TopicId");

            migrationBuilder.RenameIndex(
                name: "IX_Comments_UserId",
                table: "Comment",
                newName: "IX_Comment_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Comments_TopicId",
                table: "Comment",
                newName: "IX_Comment_TopicId");

            migrationBuilder.RenameIndex(
                name: "IX_Categories_UserId",
                table: "Category",
                newName: "IX_Category_UserId");

            migrationBuilder.AlterColumn<int>(
                name: "CategoryId",
                table: "Topic",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Topic",
                table: "Topic",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TopicLike",
                table: "TopicLike",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Comment",
                table: "Comment",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Category",
                table: "Category",
                column: "Id");

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
                name: "FK_Comment_Topic_TopicId",
                table: "Comment",
                column: "TopicId",
                principalTable: "Topic",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Topic_AspNetUsers_UserId",
                table: "Topic",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Topic_Category_CategoryId",
                table: "Topic",
                column: "CategoryId",
                principalTable: "Category",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TopicLike_AspNetUsers_UserId",
                table: "TopicLike",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TopicLike_Topic_TopicId",
                table: "TopicLike",
                column: "TopicId",
                principalTable: "Topic",
                principalColumn: "Id");
        }
    }
}
