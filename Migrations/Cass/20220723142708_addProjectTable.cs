using Microsoft.EntityFrameworkCore.Migrations;

namespace cass.Migrations.Cass
{
    public partial class addProjectTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Project",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: false),
                    Description = table.Column<string>(nullable: false),
                    Price = table.Column<float>(nullable: false),
                    Location = table.Column<string>(nullable: false),
                    Status = table.Column<string>(nullable: true),
                    Constructor_Id = table.Column<string>(nullable: true),
                    Constructor_Name = table.Column<string>(nullable: true),
                    Customer_Id = table.Column<string>(nullable: true),
                    Customer_Name = table.Column<string>(nullable: true),
                    ImageUrl = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Project", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Project");
        }
    }
}
