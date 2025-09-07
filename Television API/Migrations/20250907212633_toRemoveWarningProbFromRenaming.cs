using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Television_API.Migrations
{
    /// <inheritdoc />
    public partial class toRemoveWarningProbFromRenaming : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Episodes_TVShows_tvShowId",
                table: "Episodes");

            migrationBuilder.RenameColumn(
                name: "passwordSalt",
                table: "Users",
                newName: "PasswordSalt");

            migrationBuilder.RenameColumn(
                name: "passwordHash",
                table: "Users",
                newName: "PasswordHash");

            migrationBuilder.RenameColumn(
                name: "username",
                table: "Users",
                newName: "Username");

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
                name: "genres",
                table: "TVShows",
                newName: "Genres");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "TVShows",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "tvShowId",
                table: "Episodes",
                newName: "TvShowId");

            migrationBuilder.RenameColumn(
                name: "season",
                table: "Episodes",
                newName: "Season");

            migrationBuilder.RenameColumn(
                name: "name",
                table: "Episodes",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "airDate",
                table: "Episodes",
                newName: "AirDate");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "Episodes",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "episode",
                table: "Episodes",
                newName: "EpisodeNumber");

            migrationBuilder.RenameIndex(
                name: "IX_Episodes_tvShowId",
                table: "Episodes",
                newName: "IX_Episodes_TvShowId");

            migrationBuilder.RenameColumn(
                name: "name",
                table: "Actors",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "deathday",
                table: "Actors",
                newName: "Deathday");

            migrationBuilder.RenameColumn(
                name: "birthday",
                table: "Actors",
                newName: "Birthday");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "Actors",
                newName: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Episodes_TVShows_TvShowId",
                table: "Episodes",
                column: "TvShowId",
                principalTable: "TVShows",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Episodes_TVShows_TvShowId",
                table: "Episodes");

            migrationBuilder.RenameColumn(
                name: "PasswordSalt",
                table: "Users",
                newName: "passwordSalt");

            migrationBuilder.RenameColumn(
                name: "PasswordHash",
                table: "Users",
                newName: "passwordHash");

            migrationBuilder.RenameColumn(
                name: "Username",
                table: "Users",
                newName: "username");

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
                name: "Genres",
                table: "TVShows",
                newName: "genres");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "TVShows",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "TvShowId",
                table: "Episodes",
                newName: "tvShowId");

            migrationBuilder.RenameColumn(
                name: "Season",
                table: "Episodes",
                newName: "season");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Episodes",
                newName: "name");

            migrationBuilder.RenameColumn(
                name: "AirDate",
                table: "Episodes",
                newName: "airDate");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Episodes",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "EpisodeNumber",
                table: "Episodes",
                newName: "episode");

            migrationBuilder.RenameIndex(
                name: "IX_Episodes_TvShowId",
                table: "Episodes",
                newName: "IX_Episodes_tvShowId");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Actors",
                newName: "name");

            migrationBuilder.RenameColumn(
                name: "Deathday",
                table: "Actors",
                newName: "deathday");

            migrationBuilder.RenameColumn(
                name: "Birthday",
                table: "Actors",
                newName: "birthday");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Actors",
                newName: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_Episodes_TVShows_tvShowId",
                table: "Episodes",
                column: "tvShowId",
                principalTable: "TVShows",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
