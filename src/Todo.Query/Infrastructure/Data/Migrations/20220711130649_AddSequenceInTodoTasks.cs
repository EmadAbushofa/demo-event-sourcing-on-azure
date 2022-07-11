using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Todo.Query.Infrastructure.Data.Migrations
{
    public partial class AddSequenceInTodoTasks : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Sequence",
                table: "Tasks",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Sequence",
                table: "Tasks");
        }
    }
}
