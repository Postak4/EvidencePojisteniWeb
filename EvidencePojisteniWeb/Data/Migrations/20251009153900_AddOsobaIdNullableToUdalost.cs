using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EvidencePojisteniWeb.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddOsobaIdNullableToUdalost : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PojistneUdalosti_Pojisteni_PojisteniId",
                table: "PojistneUdalosti");

            migrationBuilder.AddColumn<int>(
                name: "OsobaId",
                table: "PojistneUdalosti",
                type: "int",
                nullable: true);

            // Pokus o naplnění z PojisteniOsoby - ideálně pojistenyc (Role = 1)
            migrationBuilder.Sql(@"
                UPDATE u
                SET u.OsobaId = po.OsobaId
                FROM PojistneUdalosti u
                OUTER APPLY (
                    SELECT TOP 1 po.OsobaId
                    FROM PojisteneOsoby po
                    WHERE po.PojisteniId = u.PojisteniId
                        AND po.Role = 1
                    ORDER BY po.Id
                ) po
                WHERE u.OsobaId IS NULL;

                UPDATE u
                SET u.OsobaId = po2.OsobaId
                FROM PojistneUdalosti u
                OUTER APPLY (
                    SELECT TOP 1 po.OsobaId
                    FROM PojisteneOsoby po
                    WHERE po.PojisteniId = u.PojisteniId
                    ORDER BY po.Id
                    ) po2
                WHERE u.OsobaId IS NULL;
            ");

            migrationBuilder.CreateIndex(
                name: "IX_PojistneUdalosti_OsobaId",
                table: "PojistneUdalosti",
                column: "OsobaId");

            migrationBuilder.AddForeignKey(
                name: "FK_PojistneUdalosti_Pojistenci_OsobaId",
                table: "PojistneUdalosti",
                column: "OsobaId",
                principalTable: "Pojistenci",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PojistneUdalosti_Pojisteni_PojisteniId",
                table: "PojistneUdalosti",
                column: "PojisteniId",
                principalTable: "Pojisteni",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PojistneUdalosti_Pojistenci_OsobaId",
                table: "PojistneUdalosti");

            migrationBuilder.DropForeignKey(
                name: "FK_PojistneUdalosti_Pojisteni_PojisteniId",
                table: "PojistneUdalosti");

            migrationBuilder.DropIndex(
                name: "IX_PojistneUdalosti_OsobaId",
                table: "PojistneUdalosti");

            migrationBuilder.DropColumn(
                name: "OsobaId",
                table: "PojistneUdalosti");

            migrationBuilder.AddForeignKey(
                name: "FK_PojistneUdalosti_Pojisteni_PojisteniId",
                table: "PojistneUdalosti",
                column: "PojisteniId",
                principalTable: "Pojisteni",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
