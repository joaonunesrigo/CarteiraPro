using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AutenticacaoUsuarios : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "UsuarioId",
                table: "Proventos",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "UsuarioId",
                table: "Operacoes",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "UsuarioId",
                table: "Ativos",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "Usuarios",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Nome = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    Email = table.Column<string>(type: "character varying(180)", maxLength: 180, nullable: false),
                    SenhaHash = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    DataCadastro = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuarios", x => x.Id);
                });

            var usuarioInicialId = new Guid("11111111-1111-1111-1111-111111111111");
            migrationBuilder.InsertData(
                table: "Usuarios",
                columns: new[] { "Id", "Nome", "Email", "SenhaHash", "DataCadastro" },
                values: new object[]
                {
                    usuarioInicialId,
                    "Usuario Inicial",
                    "admin@carteirapro.local",
                    "PBKDF2$100000$jqGaQl+mVv9LAACOi41CUQ==$eLVc0e0bU/T+JuaciNZsPEzTZw4pDucrphFEywsgxR4=",
                    new DateTime(2026, 5, 21, 0, 0, 0, DateTimeKind.Utc)
                });

            migrationBuilder.Sql("""
                UPDATE "Ativos" SET "UsuarioId" = '11111111-1111-1111-1111-111111111111'
                WHERE "UsuarioId" = '00000000-0000-0000-0000-000000000000';
                """);

            migrationBuilder.Sql("""
                UPDATE "Operacoes" SET "UsuarioId" = '11111111-1111-1111-1111-111111111111'
                WHERE "UsuarioId" = '00000000-0000-0000-0000-000000000000';
                """);

            migrationBuilder.Sql("""
                UPDATE "Proventos" SET "UsuarioId" = '11111111-1111-1111-1111-111111111111'
                WHERE "UsuarioId" = '00000000-0000-0000-0000-000000000000';
                """);

            migrationBuilder.CreateIndex(
                name: "IX_Proventos_UsuarioId",
                table: "Proventos",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_Operacoes_UsuarioId",
                table: "Operacoes",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_Ativos_UsuarioId_Ticker",
                table: "Ativos",
                columns: new[] { "UsuarioId", "Ticker" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_Email",
                table: "Usuarios",
                column: "Email",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Ativos_Usuarios_UsuarioId",
                table: "Ativos",
                column: "UsuarioId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Operacoes_Usuarios_UsuarioId",
                table: "Operacoes",
                column: "UsuarioId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Proventos_Usuarios_UsuarioId",
                table: "Proventos",
                column: "UsuarioId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Ativos_Usuarios_UsuarioId",
                table: "Ativos");

            migrationBuilder.DropForeignKey(
                name: "FK_Operacoes_Usuarios_UsuarioId",
                table: "Operacoes");

            migrationBuilder.DropForeignKey(
                name: "FK_Proventos_Usuarios_UsuarioId",
                table: "Proventos");

            migrationBuilder.DropTable(
                name: "Usuarios");

            migrationBuilder.DropIndex(
                name: "IX_Proventos_UsuarioId",
                table: "Proventos");

            migrationBuilder.DropIndex(
                name: "IX_Operacoes_UsuarioId",
                table: "Operacoes");

            migrationBuilder.DropIndex(
                name: "IX_Ativos_UsuarioId_Ticker",
                table: "Ativos");

            migrationBuilder.DropColumn(
                name: "UsuarioId",
                table: "Proventos");

            migrationBuilder.DropColumn(
                name: "UsuarioId",
                table: "Operacoes");

            migrationBuilder.DropColumn(
                name: "UsuarioId",
                table: "Ativos");
        }
    }
}
