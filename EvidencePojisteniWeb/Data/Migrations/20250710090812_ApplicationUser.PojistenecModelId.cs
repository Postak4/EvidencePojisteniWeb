using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EvidencePojisteniWeb.Data.Migrations
{
    /// <inheritdoc />
    public partial class ApplicationUserPojistenecModelId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PojistenecModelId",
                table: "AspNetUsers",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_PojistenecModelId",
                table: "AspNetUsers",
                column: "PojistenecModelId",
                unique: true,
                filter: "[PojistenecModelId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Pojistenci_PojistenecModelId",
                table: "AspNetUsers",
                column: "PojistenecModelId",
                principalTable: "Pojistenci",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Pojistenci_PojistenecModelId",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_PojistenecModelId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "PojistenecModelId",
                table: "AspNetUsers");
        }
    }
}
