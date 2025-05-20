using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EvidencePojisteniWeb.Data.Migrations
{
    /// <inheritdoc />
    public partial class PridaniPlatnostiDoPojisteniOsoby : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "PlatnostDo",
                table: "PojisteneOsoby",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1));

            migrationBuilder.AddColumn<DateTime>(
                name: "PlatnostOd",
                table: "PojisteneOsoby",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1));

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PlatnostDo",
                table: "PojisteneOsoby");

            migrationBuilder.DropColumn(
                name: "PlatnostOd",
                table: "PojisteneOsoby");

        }
    }
}
