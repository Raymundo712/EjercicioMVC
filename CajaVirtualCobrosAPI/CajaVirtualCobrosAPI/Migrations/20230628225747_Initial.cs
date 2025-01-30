using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CajaVirtualCobrosAPI.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Conceptos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Valor = table.Column<decimal>(type: "Money", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Conceptos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Clientes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    NumeroCliente = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: true),
                    Usuario_Id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clientes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EstadosCuenta",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Saldo = table.Column<decimal>(type: "Money", nullable: false),
                    Cliente_Id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EstadosCuenta", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EstadosCuenta_Clientes_Cliente_Id",
                        column: x => x.Cliente_Id,
                        principalTable: "Clientes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Movimientos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EstadoCuenta_Id = table.Column<int>(type: "int", nullable: false),
                    FechaMovimiento = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ConceptoCobro_Id = table.Column<int>(type: "int", nullable: true),
                    Abono = table.Column<decimal>(type: "Money", nullable: false),
                    EstadoCuentaId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Movimientos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Movimientos_Conceptos_ConceptoCobro_Id",
                        column: x => x.ConceptoCobro_Id,
                        principalTable: "Conceptos",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Movimientos_EstadosCuenta_EstadoCuentaId",
                        column: x => x.EstadoCuentaId,
                        principalTable: "EstadosCuenta",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Movimientos_EstadosCuenta_EstadoCuenta_Id",
                        column: x => x.EstadoCuenta_Id,
                        principalTable: "EstadosCuenta",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    UsuarioId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Usuarios",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UsuarioNombre = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: true),
                    Contrasena = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: true),
                    Rol_Id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuarios", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Usuarios_Roles_Rol_Id",
                        column: x => x.Rol_Id,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Clientes_Usuario_Id",
                table: "Clientes",
                column: "Usuario_Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EstadosCuenta_Cliente_Id",
                table: "EstadosCuenta",
                column: "Cliente_Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Movimientos_ConceptoCobro_Id",
                table: "Movimientos",
                column: "ConceptoCobro_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Movimientos_EstadoCuenta_Id",
                table: "Movimientos",
                column: "EstadoCuenta_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Movimientos_EstadoCuentaId",
                table: "Movimientos",
                column: "EstadoCuentaId");

            migrationBuilder.CreateIndex(
                name: "IX_Roles_UsuarioId",
                table: "Roles",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_Rol_Id",
                table: "Usuarios",
                column: "Rol_Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Clientes_Usuarios_Usuario_Id",
                table: "Clientes",
                column: "Usuario_Id",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Roles_Usuarios_UsuarioId",
                table: "Roles",
                column: "UsuarioId",
                principalTable: "Usuarios",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Roles_Usuarios_UsuarioId",
                table: "Roles");

            migrationBuilder.DropTable(
                name: "Movimientos");

            migrationBuilder.DropTable(
                name: "Conceptos");

            migrationBuilder.DropTable(
                name: "EstadosCuenta");

            migrationBuilder.DropTable(
                name: "Clientes");

            migrationBuilder.DropTable(
                name: "Usuarios");

            migrationBuilder.DropTable(
                name: "Roles");
        }
    }
}
