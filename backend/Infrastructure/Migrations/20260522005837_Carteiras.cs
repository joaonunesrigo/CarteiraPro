using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Carteiras : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Ativos_UsuarioId_Ticker",
                table: "Ativos");

            migrationBuilder.AddColumn<Guid>(
                name: "CarteiraId",
                table: "Proventos",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CarteiraId",
                table: "Operacoes",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CarteiraId",
                table: "Ativos",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Carteiras",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UsuarioId = table.Column<Guid>(type: "uuid", nullable: false),
                    Nome = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Descricao = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Moeda = table.Column<int>(type: "integer", nullable: false),
                    Padrao = table.Column<bool>(type: "boolean", nullable: false),
                    DataCadastro = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Carteiras", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Carteiras_Usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.Sql("""
                INSERT INTO "Carteiras" ("Id", "UsuarioId", "Nome", "Descricao", "Moeda", "Padrao", "DataCadastro")
                SELECT gen_random_uuid(), u."Id", 'Minha Carteira', NULL, 0, TRUE, now()
                FROM "Usuarios" u
                WHERE NOT EXISTS (
                    SELECT 1
                    FROM "Carteiras" c
                    WHERE c."UsuarioId" = u."Id"
                );

                UPDATE "Ativos" a
                SET "CarteiraId" = c."Id"
                FROM "Carteiras" c
                WHERE c."UsuarioId" = a."UsuarioId"
                  AND c."Padrao" = TRUE
                  AND a."CarteiraId" IS NULL;

                UPDATE "Operacoes" o
                SET "CarteiraId" = c."Id"
                FROM "Carteiras" c
                WHERE c."UsuarioId" = o."UsuarioId"
                  AND c."Padrao" = TRUE
                  AND o."CarteiraId" IS NULL;

                UPDATE "Proventos" p
                SET "CarteiraId" = c."Id"
                FROM "Carteiras" c
                WHERE c."UsuarioId" = p."UsuarioId"
                  AND c."Padrao" = TRUE
                  AND p."CarteiraId" IS NULL;
                """);

            migrationBuilder.AlterColumn<Guid>(
                name: "CarteiraId",
                table: "Proventos",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "CarteiraId",
                table: "Operacoes",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "CarteiraId",
                table: "Ativos",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Proventos_CarteiraId",
                table: "Proventos",
                column: "CarteiraId");

            migrationBuilder.CreateIndex(
                name: "IX_Operacoes_CarteiraId",
                table: "Operacoes",
                column: "CarteiraId");

            migrationBuilder.CreateIndex(
                name: "IX_Ativos_CarteiraId_Ticker",
                table: "Ativos",
                columns: new[] { "CarteiraId", "Ticker" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Ativos_UsuarioId",
                table: "Ativos",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_Carteiras_UsuarioId_Nome",
                table: "Carteiras",
                columns: new[] { "UsuarioId", "Nome" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Carteiras_UsuarioId_Padrao",
                table: "Carteiras",
                columns: new[] { "UsuarioId", "Padrao" });

            migrationBuilder.AddForeignKey(
                name: "FK_Ativos_Carteiras_CarteiraId",
                table: "Ativos",
                column: "CarteiraId",
                principalTable: "Carteiras",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Operacoes_Carteiras_CarteiraId",
                table: "Operacoes",
                column: "CarteiraId",
                principalTable: "Carteiras",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Proventos_Carteiras_CarteiraId",
                table: "Proventos",
                column: "CarteiraId",
                principalTable: "Carteiras",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Ativos_Carteiras_CarteiraId",
                table: "Ativos");

            migrationBuilder.DropForeignKey(
                name: "FK_Operacoes_Carteiras_CarteiraId",
                table: "Operacoes");

            migrationBuilder.DropForeignKey(
                name: "FK_Proventos_Carteiras_CarteiraId",
                table: "Proventos");

            migrationBuilder.DropTable(
                name: "Carteiras");

            migrationBuilder.DropIndex(
                name: "IX_Proventos_CarteiraId",
                table: "Proventos");

            migrationBuilder.DropIndex(
                name: "IX_Operacoes_CarteiraId",
                table: "Operacoes");

            migrationBuilder.DropIndex(
                name: "IX_Ativos_CarteiraId_Ticker",
                table: "Ativos");

            migrationBuilder.DropIndex(
                name: "IX_Ativos_UsuarioId",
                table: "Ativos");

            migrationBuilder.DropColumn(
                name: "CarteiraId",
                table: "Proventos");

            migrationBuilder.DropColumn(
                name: "CarteiraId",
                table: "Operacoes");

            migrationBuilder.DropColumn(
                name: "CarteiraId",
                table: "Ativos");

            migrationBuilder.CreateIndex(
                name: "IX_Ativos_UsuarioId_Ticker",
                table: "Ativos",
                columns: new[] { "UsuarioId", "Ticker" },
                unique: true);
        }
    }
}
