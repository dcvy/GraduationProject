using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kclinic.DataAccess.Migrations
{
    public partial class AddTransactionHashToOrderHeader : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TransactionHash",
                table: "OrderHeaders",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TransactionHash",
                table: "OrderHeaders");
        }
    }
}
