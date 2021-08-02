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

            migrationBuilder.CreateTable(
                name: "WrapperClasses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TypeId = table.Column<int>(type: "INTEGER", nullable: false),
                    ExampleId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WrapperClasses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WrapperClasses_Examples_ExampleId_TypeId",
                        columns: x => new { x.ExampleId, x.TypeId },
                        principalTable: "Examples",
                        principalColumns: new[] { "Id", "TypeId" },
                        onDelete: ReferentialAction.Cascade);
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
                values: new object[] { 2, 4 });

            migrationBuilder.InsertData(
                table: "WrapperClasses",
                columns: new[] { "Id", "ExampleId", "TypeId" },
                values: new object[] { 5, 2, 4 });

            migrationBuilder.CreateIndex(
                name: "IX_WrapperClasses_ExampleId_TypeId",
                table: "WrapperClasses",
                columns: new[] { "ExampleId", "TypeId" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WrapperClasses");

            migrationBuilder.DropTable(
                name: "Examples");
        }
    }
}
