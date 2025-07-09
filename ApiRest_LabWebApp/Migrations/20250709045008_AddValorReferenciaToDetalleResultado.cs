using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ApiRest_LabWebApp.Migrations
{
    /// <inheritdoc />
    public partial class AddValorReferenciaToDetalleResultado : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "total",
                table: "orden",
                type: "decimal(10,2)",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "decimal(10,2)",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "id_paciente",
                table: "orden",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<string>(
                name: "valor_referencia",
                table: "detalle_resultado",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "IdOrdenNavigationIdOrden",
                table: "detalle_convenio",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_detalle_convenio_IdOrdenNavigationIdOrden",
                table: "detalle_convenio",
                column: "IdOrdenNavigationIdOrden");

            migrationBuilder.AddForeignKey(
                name: "FK_detalle_convenio_orden_IdOrdenNavigationIdOrden",
                table: "detalle_convenio",
                column: "IdOrdenNavigationIdOrden",
                principalTable: "orden",
                principalColumn: "id_orden",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_detalle_convenio_orden_IdOrdenNavigationIdOrden",
                table: "detalle_convenio");

            migrationBuilder.DropIndex(
                name: "IX_detalle_convenio_IdOrdenNavigationIdOrden",
                table: "detalle_convenio");

            migrationBuilder.DropColumn(
                name: "valor_referencia",
                table: "detalle_resultado");

            migrationBuilder.DropColumn(
                name: "IdOrdenNavigationIdOrden",
                table: "detalle_convenio");

            migrationBuilder.AlterColumn<decimal>(
                name: "total",
                table: "orden",
                type: "decimal(10,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(10,2)");

            migrationBuilder.AlterColumn<int>(
                name: "id_paciente",
                table: "orden",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);
        }
    }
}
