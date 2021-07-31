using Microsoft.EntityFrameworkCore.Migrations;

namespace hotchocolate_playground.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Examples",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false),
                    TypeId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Examples", x => new { x.Id, x.TypeId });
                });

            migrationBuilder.InsertData(
                table: "Examples",
                columns: new[] { "Id", "TypeId" },
                values: new object[] { 1, 1 });

            migrationBuilder.InsertData(
                table: "Examples",
                columns: new[] { "Id", "TypeId" },
                values: new object[] { 1, 2 });

            migrationBuilder.InsertData(
                table: "Examples",
                columns: new[] { "Id", "TypeId" },
                values: new object[] { 1, 3 });

            migrationBuilder.InsertData(
                table: "Examples",
                columns: new[] { "Id", "TypeId" },
                values: new object[] { 1, 4 });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Examples");
        }
    }
}
