using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LegoInventoryHelper.Migrations
{
    public partial class AddedStuff : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SetID",
                table: "Prices",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SetID",
                table: "Prices");
        }
    }
}
