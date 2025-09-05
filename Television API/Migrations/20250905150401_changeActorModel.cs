using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Television_API.Migrations
{
    /// <inheritdoc />
    public partial class changeActorModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "age",
                table: "Actors");

            migrationBuilder.AddColumn<string>(
                name: "birthday",
                table: "Actors",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "country",
                table: "Actors",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "deathday",
                table: "Actors",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "birthday",
                table: "Actors");

            migrationBuilder.DropColumn(
                name: "country",
                table: "Actors");

            migrationBuilder.DropColumn(
                name: "deathday",
                table: "Actors");

            migrationBuilder.AddColumn<int>(
                name: "age",
                table: "Actors",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }
    }
}
