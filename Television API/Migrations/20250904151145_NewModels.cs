using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Television_API.Migrations
{
    /// <inheritdoc />
    public partial class NewModels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Title",
                table: "TVShows",
                newName: "title");

            migrationBuilder.RenameColumn(
                name: "StartDate",
                table: "TVShows",
                newName: "startDate");

            migrationBuilder.RenameColumn(
                name: "IsOngoing",
                table: "TVShows",
                newName: "isOngoing");

            migrationBuilder.RenameColumn(
                name: "Genre",
                table: "TVShows",
                newName: "genre");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "TVShows",
                newName: "id");

            migrationBuilder.CreateTable(
                name: "Actors",
                columns: table => new
                {
                    id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    name = table.Column<string>(type: "TEXT", nullable: false),
                    age = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Actors", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Episodes",
                columns: table => new
                {
                    id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    season = table.Column<int>(type: "INTEGER", nullable: false),
                    episode = table.Column<int>(type: "INTEGER", nullable: false),
                    name = table.Column<string>(type: "TEXT", nullable: false),
                    air_date = table.Column<string>(type: "TEXT", nullable: false),
                    tvShowId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Episodes", x => x.id);
                    table.ForeignKey(
                        name: "FK_Episodes_TVShows_tvShowId",
                        column: x => x.tvShowId,
                        principalTable: "TVShows",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TVShowActor",
                columns: table => new
                {
                    ActorId = table.Column<int>(type: "INTEGER", nullable: false),
                    TVShowId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TVShowActor", x => new { x.ActorId, x.TVShowId });
                    table.ForeignKey(
                        name: "FK_TVShowActor_Actors_ActorId",
                        column: x => x.ActorId,
                        principalTable: "Actors",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TVShowActor_TVShows_TVShowId",
                        column: x => x.TVShowId,
                        principalTable: "TVShows",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Episodes_tvShowId",
                table: "Episodes",
                column: "tvShowId");

            migrationBuilder.CreateIndex(
                name: "IX_TVShowActor_TVShowId",
                table: "TVShowActor",
                column: "TVShowId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Episodes");

            migrationBuilder.DropTable(
                name: "TVShowActor");

            migrationBuilder.DropTable(
                name: "Actors");

            migrationBuilder.RenameColumn(
                name: "title",
                table: "TVShows",
                newName: "Title");

            migrationBuilder.RenameColumn(
                name: "startDate",
                table: "TVShows",
                newName: "StartDate");

            migrationBuilder.RenameColumn(
                name: "isOngoing",
                table: "TVShows",
                newName: "IsOngoing");

            migrationBuilder.RenameColumn(
                name: "genre",
                table: "TVShows",
                newName: "Genre");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "TVShows",
                newName: "Id");
        }
    }
}
