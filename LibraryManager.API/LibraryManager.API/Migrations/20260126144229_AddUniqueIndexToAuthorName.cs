using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LibraryManager.API.Migrations
{
    /// <inheritdoc />
    public partial class AddUniqueIndexToAuthorName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:CollationDefinition:ignore_case_and_accents", "en-u-ks-level1,en-u-ks-level1,icu,False");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Authors",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                collation: "ignore_case_and_accents",
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.CreateIndex(
                name: "IX_Authors_Name",
                table: "Authors",
                column: "Name",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Authors_Name",
                table: "Authors");

            migrationBuilder.AlterDatabase()
                .OldAnnotation("Npgsql:CollationDefinition:ignore_case_and_accents", "en-u-ks-level1,en-u-ks-level1,icu,False");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Authors",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100,
                oldCollation: "ignore_case_and_accents");
        }
    }
}
