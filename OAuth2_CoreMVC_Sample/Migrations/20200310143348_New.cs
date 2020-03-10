using Microsoft.EntityFrameworkCore.Migrations;

namespace OAuth2_CoreMVC_Sample.Migrations
{
    public partial class New : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                "Token",
                table => new
                {
                    RealmId = table.Column<string>(maxLength: 50),
                    AccessToken = table.Column<string>(maxLength: 1000),
                    RefreshToken = table.Column<string>(maxLength: 1000)
                },
                constraints: table => { table.PrimaryKey("PK_Token", x => x.RealmId); });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                "Token");
        }
    }
}