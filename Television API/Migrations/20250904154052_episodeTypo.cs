using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Television_API.Migrations
{
    /// <inheritdoc />
    public partial class episodeTypo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "air_date",
                table: "Episodes",
                newName: "airDate");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "airDate",
                table: "Episodes",
                newName: "air_date");
        }
    }
}
