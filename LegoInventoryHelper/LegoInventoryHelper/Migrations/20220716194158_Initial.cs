using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LegoInventoryHelper.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Themes",
                columns: table => new
                {
                    ID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    ThemeID = table.Column<int>(type: "INTEGER", nullable: false),
                    ParentID = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Themes", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "LegoInventoryItems",
                columns: table => new
                {
                    ID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    NumberOfParts = table.Column<int>(type: "INTEGER", nullable: false),
                    ImageURL = table.Column<string>(type: "TEXT", nullable: false),
                    SetID = table.Column<string>(type: "TEXT", nullable: false),
                    ThemeID = table.Column<int>(type: "INTEGER", nullable: false),
                    YearOfRelease = table.Column<int>(type: "INTEGER", nullable: false),
                    PriceBought = table.Column<double>(type: "REAL", nullable: false),
                    PriceSold = table.Column<double>(type: "REAL", nullable: false),
                    IsSellable = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LegoInventoryItems", x => x.ID);
                    table.ForeignKey(
                        name: "FK_LegoInventoryItems_Themes_ThemeID",
                        column: x => x.ThemeID,
                        principalTable: "Themes",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Prices",
                columns: table => new
                {
                    ID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    MinPrice = table.Column<double>(type: "REAL", nullable: false),
                    MaxPrice = table.Column<double>(type: "REAL", nullable: false),
                    AveragePrice = table.Column<double>(type: "REAL", nullable: false),
                    QuantityAveragePrice = table.Column<double>(type: "REAL", nullable: false),
                    RequestDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    LegoInventoryItemID = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Prices", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Prices_LegoInventoryItems_LegoInventoryItemID",
                        column: x => x.LegoInventoryItemID,
                        principalTable: "LegoInventoryItems",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateIndex(
                name: "IX_LegoInventoryItems_ThemeID",
                table: "LegoInventoryItems",
                column: "ThemeID");

            migrationBuilder.CreateIndex(
                name: "IX_Prices_LegoInventoryItemID",
                table: "Prices",
                column: "LegoInventoryItemID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Prices");

            migrationBuilder.DropTable(
                name: "LegoInventoryItems");

            migrationBuilder.DropTable(
                name: "Themes");
        }
    }
}
