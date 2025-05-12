using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EvidencePojisteniWeb.Data.Migrations
{
    /// <inheritdoc />
    public partial class FixCascadePathsMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "AspNetUserTokens",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldMaxLength: 128);

            migrationBuilder.AlterColumn<string>(
                name: "LoginProvider",
                table: "AspNetUserTokens",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldMaxLength: 128);

            migrationBuilder.AlterColumn<string>(
                name: "ProviderKey",
                table: "AspNetUserLogins",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldMaxLength: 128);

            migrationBuilder.AlterColumn<string>(
                name: "LoginProvider",
                table: "AspNetUserLogins",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldMaxLength: 128);

            migrationBuilder.CreateTable(
                name: "Pojistenci",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Jmeno = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Prijmeni = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    DatumNarozeni = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UliceCpCe = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Mesto = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    PSC = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Stat = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Telefon = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pojistenci", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Pojisteni",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PojistenecId = table.Column<int>(type: "int", nullable: false),
                    TypPojisteni = table.Column<int>(type: "int", nullable: false),
                    PredmetPojisteni = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    DatumZacatku = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DatumKonce = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Castka = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pojisteni", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Pojisteni_Pojistenci_PojistenecId",
                        column: x => x.PojistenecId,
                        principalTable: "Pojistenci",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PojisteneOsoby",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OsobaId = table.Column<int>(type: "int", nullable: false),
                    PojisteniId = table.Column<int>(type: "int", nullable: false),
                    Role = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PojisteneOsoby", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PojisteneOsoby_Pojistenci_OsobaId",
                        column: x => x.OsobaId,
                        principalTable: "Pojistenci",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PojisteneOsoby_Pojisteni_PojisteniId",
                        column: x => x.PojisteniId,
                        principalTable: "Pojisteni",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PojistneUdalosti",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PopisUdalosti = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    DatumUdalosti = table.Column<DateTime>(type: "datetime2", nullable: false),
                    MistoUdalosti = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Skoda = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Vyreseno = table.Column<bool>(type: "bit", nullable: false),
                    Svedek = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: true),
                    AdresaSvedka = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Poznamka = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    PojisteniId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PojistneUdalosti", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PojistneUdalosti_Pojisteni_PojisteniId",
                        column: x => x.PojisteniId,
                        principalTable: "Pojisteni",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PojisteneOsoby_OsobaId",
                table: "PojisteneOsoby",
                column: "OsobaId");

            migrationBuilder.CreateIndex(
                name: "IX_PojisteneOsoby_PojisteniId",
                table: "PojisteneOsoby",
                column: "PojisteniId");

            migrationBuilder.CreateIndex(
                name: "IX_Pojisteni_PojistenecId",
                table: "Pojisteni",
                column: "PojistenecId");

            migrationBuilder.CreateIndex(
                name: "IX_PojistneUdalosti_PojisteniId",
                table: "PojistneUdalosti",
                column: "PojisteniId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PojisteneOsoby");

            migrationBuilder.DropTable(
                name: "PojistneUdalosti");

            migrationBuilder.DropTable(
                name: "Pojisteni");

            migrationBuilder.DropTable(
                name: "Pojistenci");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "AspNetUserTokens",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "LoginProvider",
                table: "AspNetUserTokens",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "ProviderKey",
                table: "AspNetUserLogins",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "LoginProvider",
                table: "AspNetUserLogins",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");
        }
    }
}
