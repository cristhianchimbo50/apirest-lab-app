using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ApiRest_LabWebApp.Migrations
{
    /// <inheritdoc />
    public partial class RemoveIdOrdenNavigationErrorFix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Nombres",
                table: "paciente",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Nombres",
                table: "paciente");
        }
    }
}
