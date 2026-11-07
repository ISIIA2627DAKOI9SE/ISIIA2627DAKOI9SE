using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AppForSEII.API.Migrations
{
    /// <inheritdoc />
    public partial class ActualizarModelosSprint2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ComprasAccesorios_AspNetUsers_ClienteId",
                table: "ComprasAccesorios");

            migrationBuilder.DropForeignKey(
                name: "FK_LineaCompraAccesorios_ComprasAccesorios_CompraAccesorioId",
                table: "LineaCompraAccesorios");

            migrationBuilder.DropForeignKey(
                name: "FK_LineasReserva_ReservaImpresoras_ReservaImpresoraId",
                table: "LineasReserva");

            migrationBuilder.DropPrimaryKey(
                name: "PK_LineasReserva",
                table: "LineasReserva");

            migrationBuilder.DropIndex(
                name: "IX_LineasReserva_ReservaImpresoraId",
                table: "LineasReserva");

            migrationBuilder.DropPrimaryKey(
                name: "PK_LineasCompraModelos",
                table: "LineasCompraModelos");

            migrationBuilder.DropIndex(
                name: "IX_LineasCompraModelos_CompraModelo3DId",
                table: "LineasCompraModelos");

            migrationBuilder.DropPrimaryKey(
                name: "PK_LineaCompraAccesorios",
                table: "LineaCompraAccesorios");

            migrationBuilder.DropIndex(
                name: "IX_LineaCompraAccesorios_CompraAccesorioId",
                table: "LineaCompraAccesorios");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ComprasAccesorios",
                table: "ComprasAccesorios");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "LineaCompraAccesorios");

            migrationBuilder.RenameTable(
                name: "ComprasAccesorios",
                newName: "CompraAccesorios");

            migrationBuilder.RenameIndex(
                name: "IX_ComprasAccesorios_ClienteId",
                table: "CompraAccesorios",
                newName: "IX_CompraAccesorios_ClienteId");

            migrationBuilder.AlterColumn<decimal>(
                name: "PrecioTotal",
                table: "ReservaImpresoras",
                type: "decimal(10,2)",
                precision: 10,
                scale: 2,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<string>(
                name: "NombreCliente",
                table: "ReservaImpresoras",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "DireccionFacturacion",
                table: "ReservaImpresoras",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "ApellidosCliente",
                table: "ReservaImpresoras",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "ClienteId",
                table: "ReservaImpresoras",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<int>(
                name: "ReservaImpresoraId",
                table: "LineasReserva",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "PrecioSubtotal",
                table: "LineasReserva",
                type: "decimal(10,2)",
                precision: 10,
                scale: 2,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "LineasReserva");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "LineasReserva",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.DropColumn(
                name: "Id",
                table: "LineasCompraModelos");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "LineasCompraModelos",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<decimal>(
                name: "PrecioReserva",
                table: "Impresora3Ds",
                type: "decimal(10,2)",
                precision: 10,
                scale: 2,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "PrecioKilovatioHora",
                table: "Impresora3Ds",
                type: "decimal(10,2)",
                precision: 10,
                scale: 2,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<string>(
                name: "Nombre",
                table: "Impresora3Ds",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Modelo",
                table: "Impresora3Ds",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Descripcion",
                table: "Impresora3Ds",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddPrimaryKey(
                name: "PK_LineasReserva",
                table: "LineasReserva",
                columns: new[] { "ReservaImpresoraId", "Impresora3DId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_LineasCompraModelos",
                table: "LineasCompraModelos",
                columns: new[] { "CompraModelo3DId", "Modelo3DId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_LineaCompraAccesorios",
                table: "LineaCompraAccesorios",
                columns: new[] { "CompraAccesorioId", "AccesorioId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_CompraAccesorios",
                table: "CompraAccesorios",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_ReservaImpresoras_ClienteId",
                table: "ReservaImpresoras",
                column: "ClienteId");

            migrationBuilder.AddForeignKey(
                name: "FK_CompraAccesorios_AspNetUsers_ClienteId",
                table: "CompraAccesorios",
                column: "ClienteId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_LineaCompraAccesorios_CompraAccesorios_CompraAccesorioId",
                table: "LineaCompraAccesorios",
                column: "CompraAccesorioId",
                principalTable: "CompraAccesorios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_LineasReserva_ReservaImpresoras_ReservaImpresoraId",
                table: "LineasReserva",
                column: "ReservaImpresoraId",
                principalTable: "ReservaImpresoras",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ReservaImpresoras_AspNetUsers_ClienteId",
                table: "ReservaImpresoras",
                column: "ClienteId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CompraAccesorios_AspNetUsers_ClienteId",
                table: "CompraAccesorios");

            migrationBuilder.DropForeignKey(
                name: "FK_LineaCompraAccesorios_CompraAccesorios_CompraAccesorioId",
                table: "LineaCompraAccesorios");

            migrationBuilder.DropForeignKey(
                name: "FK_LineasReserva_ReservaImpresoras_ReservaImpresoraId",
                table: "LineasReserva");

            migrationBuilder.DropForeignKey(
                name: "FK_ReservaImpresoras_AspNetUsers_ClienteId",
                table: "ReservaImpresoras");

            migrationBuilder.DropIndex(
                name: "IX_ReservaImpresoras_ClienteId",
                table: "ReservaImpresoras");

            migrationBuilder.DropPrimaryKey(
                name: "PK_LineasReserva",
                table: "LineasReserva");

            migrationBuilder.DropPrimaryKey(
                name: "PK_LineasCompraModelos",
                table: "LineasCompraModelos");

            migrationBuilder.DropPrimaryKey(
                name: "PK_LineaCompraAccesorios",
                table: "LineaCompraAccesorios");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CompraAccesorios",
                table: "CompraAccesorios");

            migrationBuilder.DropColumn(
                name: "ClienteId",
                table: "ReservaImpresoras");

            migrationBuilder.RenameTable(
                name: "CompraAccesorios",
                newName: "ComprasAccesorios");

            migrationBuilder.RenameIndex(
                name: "IX_CompraAccesorios_ClienteId",
                table: "ComprasAccesorios",
                newName: "IX_ComprasAccesorios_ClienteId");

            migrationBuilder.AlterColumn<decimal>(
                name: "PrecioTotal",
                table: "ReservaImpresoras",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(10,2)",
                oldPrecision: 10,
                oldScale: 2);

            migrationBuilder.AlterColumn<string>(
                name: "NombreCliente",
                table: "ReservaImpresoras",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "DireccionFacturacion",
                table: "ReservaImpresoras",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<string>(
                name: "ApellidosCliente",
                table: "ReservaImpresoras",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<decimal>(
                name: "PrecioSubtotal",
                table: "LineasReserva",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(10,2)",
                oldPrecision: 10,
                oldScale: 2);

            migrationBuilder.DropColumn(
                name: "Id",
                table: "LineasReserva");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "LineasReserva",
                type: "int",
                nullable: false)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AlterColumn<int>(
                name: "ReservaImpresoraId",
                table: "LineasReserva",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "LineasCompraModelos");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "LineasCompraModelos",
                type: "int",
                nullable: false)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "LineaCompraAccesorios",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AlterColumn<decimal>(
                name: "PrecioReserva",
                table: "Impresora3Ds",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(10,2)",
                oldPrecision: 10,
                oldScale: 2);

            migrationBuilder.AlterColumn<decimal>(
                name: "PrecioKilovatioHora",
                table: "Impresora3Ds",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(10,2)",
                oldPrecision: 10,
                oldScale: 2);

            migrationBuilder.AlterColumn<string>(
                name: "Nombre",
                table: "Impresora3Ds",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Modelo",
                table: "Impresora3Ds",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Descripcion",
                table: "Impresora3Ds",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(1000)",
                oldMaxLength: 1000);

            migrationBuilder.AddPrimaryKey(
                name: "PK_LineasReserva",
                table: "LineasReserva",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_LineasCompraModelos",
                table: "LineasCompraModelos",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_LineaCompraAccesorios",
                table: "LineaCompraAccesorios",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ComprasAccesorios",
                table: "ComprasAccesorios",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_LineasReserva_ReservaImpresoraId",
                table: "LineasReserva",
                column: "ReservaImpresoraId");

            migrationBuilder.CreateIndex(
                name: "IX_LineasCompraModelos_CompraModelo3DId",
                table: "LineasCompraModelos",
                column: "CompraModelo3DId");

            migrationBuilder.CreateIndex(
                name: "IX_LineaCompraAccesorios_CompraAccesorioId",
                table: "LineaCompraAccesorios",
                column: "CompraAccesorioId");

            migrationBuilder.AddForeignKey(
                name: "FK_ComprasAccesorios_AspNetUsers_ClienteId",
                table: "ComprasAccesorios",
                column: "ClienteId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_LineaCompraAccesorios_ComprasAccesorios_CompraAccesorioId",
                table: "LineaCompraAccesorios",
                column: "CompraAccesorioId",
                principalTable: "ComprasAccesorios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_LineasReserva_ReservaImpresoras_ReservaImpresoraId",
                table: "LineasReserva",
                column: "ReservaImpresoraId",
                principalTable: "ReservaImpresoras",
                principalColumn: "Id");
        }
    }
}
