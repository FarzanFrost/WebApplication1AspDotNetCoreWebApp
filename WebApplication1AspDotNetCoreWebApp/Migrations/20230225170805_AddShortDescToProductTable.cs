using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApplication1AspDotNetCoreWebApp.Migrations
{
    public partial class AddShortDescToProductTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "shortDesc",
                table: "Product",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "shortDesc",
                table: "Product");
        }
    }
}
