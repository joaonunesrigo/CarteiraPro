using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class HistoricoOperacoes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Operacoes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AtivoId = table.Column<Guid>(type: "uuid", nullable: false),
                    Tipo = table.Column<int>(type: "integer", nullable: false),
                    Data = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Quantidade = table.Column<decimal>(type: "numeric(18,8)", precision: 18, scale: 8, nullable: false),
                    PrecoUnitario = table.Column<decimal>(type: "numeric(18,6)", precision: 18, scale: 6, nullable: false),
                    Taxas = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    Observacao = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    DataCadastro = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Operacoes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Operacoes_Ativos_AtivoId",
                        column: x => x.AtivoId,
                        principalTable: "Ativos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Operacoes_AtivoId",
                table: "Operacoes",
                column: "AtivoId");

            migrationBuilder.Sql("""
                INSERT INTO "Operacoes" (
                    "Id",
                    "AtivoId",
                    "Tipo",
                    "Data",
                    "Quantidade",
                    "PrecoUnitario",
                    "Taxas",
                    "Observacao",
                    "DataCadastro"
                )
                SELECT
                    "Id",
                    "Id",
                    0,
                    date_trunc('day', "DataCadastro" AT TIME ZONE 'UTC') AT TIME ZONE 'UTC',
                    "Quantidade",
                    "PrecoMedio",
                    0,
                    'Posicao inicial migrada do cadastro de ativo.',
                    "DataCadastro"
                FROM "Ativos"
                WHERE "Quantidade" > 0;
                """);

            migrationBuilder.DropColumn(
                name: "PrecoMedio",
                table: "Ativos");

            migrationBuilder.DropColumn(
                name: "Quantidade",
                table: "Ativos");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "PrecoMedio",
                table: "Ativos",
                type: "numeric(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Quantidade",
                table: "Ativos",
                type: "numeric(18,8)",
                precision: 18,
                scale: 8,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.Sql("""
                UPDATE "Ativos" AS a
                SET
                    "Quantidade" = COALESCE(pos."Quantidade", 0),
                    "PrecoMedio" = COALESCE(pos."PrecoMedio", 0)
                FROM (
                    SELECT
                        "AtivoId",
                        SUM(CASE WHEN "Tipo" = 0 THEN "Quantidade" ELSE -"Quantidade" END) AS "Quantidade",
                        CASE
                            WHEN SUM(CASE WHEN "Tipo" = 0 THEN "Quantidade" ELSE 0 END) > 0
                            THEN SUM(CASE WHEN "Tipo" = 0 THEN ("Quantidade" * "PrecoUnitario") + "Taxas" ELSE 0 END)
                                / SUM(CASE WHEN "Tipo" = 0 THEN "Quantidade" ELSE 0 END)
                            ELSE 0
                        END AS "PrecoMedio"
                    FROM "Operacoes"
                    GROUP BY "AtivoId"
                ) AS pos
                WHERE a."Id" = pos."AtivoId";
                """);

            migrationBuilder.DropTable(
                name: "Operacoes");
        }
    }
}
