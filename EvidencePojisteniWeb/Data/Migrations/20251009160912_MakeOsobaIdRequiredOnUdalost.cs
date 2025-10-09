using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EvidencePojisteniWeb.Data.Migrations
{
    /// <inheritdoc />
    public partial class MakeOsobaIdRequiredOnUdalost : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
            name: "OsobaId",
            table: "PojistneUdalosti",
            type: "int",
            nullable: false,
            oldClrType: typeof(int),
            oldType: "int",
            oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
