using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Television_API.Migrations
{
    /// <inheritdoc />
    public partial class addedUsersFavorites : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserFavoriteShow",
                columns: table => new
                {
                    TVShowId = table.Column<int>(type: "INTEGER", nullable: false),
                    Username = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserFavoriteShow", x => new { x.TVShowId, x.Username });
                    table.ForeignKey(
                        name: "FK_UserFavoriteShow_TVShows_TVShowId",
                        column: x => x.TVShowId,
                        principalTable: "TVShows",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserFavoriteShow_Users_Username",
                        column: x => x.Username,
                        principalTable: "Users",
                        principalColumn: "username",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserFavoriteShow_Username",
                table: "UserFavoriteShow",
                column: "Username");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserFavoriteShow");
        }
    }
}
