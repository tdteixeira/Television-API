using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Television_API.Migrations
{
    /// <inheritdoc />
    public partial class addedUsers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    username = table.Column<string>(type: "TEXT", nullable: false),
                    passwordHash = table.Column<byte[]>(type: "BLOB", nullable: false),
                    passwordSalt = table.Column<byte[]>(type: "BLOB", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.username);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
