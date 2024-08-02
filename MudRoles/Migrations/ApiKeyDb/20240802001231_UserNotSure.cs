using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MudRoles.Migrations.ApiKeyDb
{
    /// <inheritdoc />
    public partial class UserNotSure : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ScopeName",
                table: "Scope",
                newName: "ScopeVerb");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ScopeVerb",
                table: "Scope",
                newName: "ScopeName");
        }
    }
}
