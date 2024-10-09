using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MudRoles.Migrations
{
    /// <inheritdoc />
    public partial class ProfileExtension : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AvatarChoice",
                table: "AspNetUsers",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "UploadedAvatar",
                table: "AspNetUsers",
                type: "BLOB",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AvatarChoice",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "UploadedAvatar",
                table: "AspNetUsers");
        }
    }
}
