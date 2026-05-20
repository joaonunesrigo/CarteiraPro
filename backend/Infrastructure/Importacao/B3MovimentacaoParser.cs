using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using Application.Exceptions;
using ClosedXML.Excel;
using Domain.Enums;
using Domain.Interfaces;
using Domain.Models;

namespace Infrastructure.Importacao;

public partial class B3MovimentacaoParser : IB3MovimentacaoParser
{
    private static readonly Dictionary<string, string> AliasesColunas = new(StringComparer.OrdinalIgnoreCase)
    {
        ["Entrada/Saída"] = "entradaSaida",
        ["Entrada/Saida"] = "entradaSaida",
        ["Data"] = "data",
        ["Pagamento"] = "data",
        ["Movimentação"] = "movimentacao",
        ["Movimentacao"] = "movimentacao",
        ["Tipo de Evento"] = "movimentacao",
        ["Produto"] = "produto",
        ["Quantidade"] = "quantidade",
        ["Preço unitário"] = "valorPorCota",
        ["Preco unitario"] = "valorPorCota",
        ["Valor da Operação"] = "valorTotal",
        ["Valor da Operacao"] = "valorTotal",
        ["Valor líquido"] = "valorTotal",
        ["Valor liquido"] = "valorTotal",
    };

    public IReadOnlyList<LinhaMovimentacaoB3> Parse(Stream arquivo)
    {
        using var workbook = new XLWorkbook(arquivo);
        var linhas = new List<LinhaMovimentacaoB3>();

        foreach (var planilha in workbook.Worksheets)
            linhas.AddRange(LerAba(planilha));

        if (linhas.Count == 0)
            throw new ArquivoB3InvalidoException(
                "Nenhum provento encontrado. Use o Excel de movimentação da B3 com lançamentos de Dividendos, JCP ou Rendimentos.");

        return linhas;
    }

    private static IEnumerable<LinhaMovimentacaoB3> LerAba(IXLWorksheet planilha)
    {
        var ultimaLinha = planilha.LastRowUsed()?.RowNumber() ?? 0;
        if (ultimaLinha == 0)
            yield break;

        var (linhaCabecalho, colunas) = EncontrarCabecalho(planilha, ultimaLinha);
        if (linhaCabecalho == 0)
            yield break;

        for (var linha = linhaCabecalho + 1; linha <= ultimaLinha; linha++)
        {
            if (EhLinhaTotal(planilha, linha))
                continue;

            var entradaSaida = LerTexto(planilha, linha, colunas, "entradaSaida");
            if (!EhCredito(entradaSaida))
                continue;

            var movimentacao = LerTexto(planilha, linha, colunas, "movimentacao");
            if (string.IsNullOrWhiteSpace(movimentacao) || !TryInferirTipo(movimentacao, out var tipo))
                continue;

            var produto = LerTexto(planilha, linha, colunas, "produto");
            var ticker = produto is null ? null : ExtrairTickerDoProduto(produto);
            if (string.IsNullOrWhiteSpace(ticker))
                continue;

            var dataPagamento = LerData(planilha, linha, colunas, "data");
            if (dataPagamento is null)
                continue;

            var quantidade = LerDecimal(planilha, linha, colunas, "quantidade");
            var valorPorCota = LerDecimal(planilha, linha, colunas, "valorPorCota");
            var valorTotal = LerDecimal(planilha, linha, colunas, "valorTotal");

            if (valorTotal is not null)
                valorTotal = Math.Abs(valorTotal.Value);

            var permiteSemQuantidade = tipo is TipoProvento.Reembolso or TipoProvento.RestituicaoDeCapital;
            if (quantidade is null or <= 0)
            {
                if (!permiteSemQuantidade || valorTotal is null or <= 0)
                    continue;

                quantidade = 1m;
                valorPorCota = valorTotal;
            }
            else if (valorTotal is > 0)
            {
                valorPorCota = valorTotal.Value / quantidade.Value;
            }
            else if (valorPorCota is > 0)
            {
                valorTotal = valorPorCota.Value * quantidade.Value;
            }

            if (valorPorCota is null or <= 0 || valorTotal is null or <= 0)
                continue;

            valorPorCota = Math.Round(valorPorCota.Value, 6, MidpointRounding.AwayFromZero);
            valorTotal = Math.Round(valorTotal.Value, 2, MidpointRounding.AwayFromZero);

            yield return new LinhaMovimentacaoB3
            {
                Ticker = ticker.ToUpperInvariant(),
                Produto = produto,
                Movimentacao = movimentacao,
                Quantidade = quantidade.Value,
                ValorPorCota = valorPorCota.Value,
                ValorTotal = valorTotal.Value,
                DataPagamento = DateTime.SpecifyKind(dataPagamento.Value.Date, DateTimeKind.Utc),
                Tipo = tipo,
                OrigemAba = planilha.Name,
            };
        }
    }

    private static (int LinhaCabecalho, Dictionary<string, int> Colunas) EncontrarCabecalho(
        IXLWorksheet planilha,
        int ultimaLinha)
    {
        var limite = Math.Min(ultimaLinha, 20);

        for (var linha = 1; linha <= limite; linha++)
        {
            var colunas = new Dictionary<string, int>();
            var ultimaColuna = planilha.Row(linha).LastCellUsed()?.Address.ColumnNumber ?? 0;

            for (var col = 1; col <= ultimaColuna; col++)
            {
                var titulo = Normalizar(planilha.Cell(linha, col).GetString().Trim());
                if (string.IsNullOrEmpty(titulo))
                    continue;

                if (AliasesColunas.TryGetValue(titulo, out var chave))
                    colunas[chave] = col;
            }

            if (colunas.ContainsKey("data")
                && colunas.ContainsKey("movimentacao")
                && colunas.ContainsKey("produto")
                && colunas.ContainsKey("quantidade"))
            {
                return (linha, colunas);
            }
        }

        return (0, new Dictionary<string, int>());
    }

    private static bool EhCredito(string? entradaSaida)
    {
        if (string.IsNullOrWhiteSpace(entradaSaida))
            return true;

        var texto = Normalizar(entradaSaida);
        return texto.Contains("credito", StringComparison.OrdinalIgnoreCase);
    }

    private static bool TryInferirTipo(string movimentacao, out TipoProvento tipo)
    {
        var texto = Normalizar(movimentacao);

        if (texto.Contains("juros", StringComparison.OrdinalIgnoreCase)
            || texto.Contains("capital proprio", StringComparison.OrdinalIgnoreCase)
            || texto.Contains("jcp", StringComparison.OrdinalIgnoreCase))
        {
            tipo = TipoProvento.JurosSobreCapitalProprio;
            return true;
        }

        if (texto.Contains("rendimento", StringComparison.OrdinalIgnoreCase))
        {
            tipo = TipoProvento.Rendimento;
            return true;
        }

        if (texto.Contains("dividendo", StringComparison.OrdinalIgnoreCase))
        {
            tipo = TipoProvento.Dividendo;
            return true;
        }

        if (texto.Contains("reembolso", StringComparison.OrdinalIgnoreCase))
        {
            tipo = TipoProvento.Reembolso;
            return true;
        }

        if (texto.Contains("restituicao", StringComparison.OrdinalIgnoreCase)
            || texto.Contains("amortizacao", StringComparison.OrdinalIgnoreCase))
        {
            tipo = TipoProvento.RestituicaoDeCapital;
            return true;
        }

        tipo = default;
        return false;
    }

    private static string? ExtrairTickerDoProduto(string produto)
    {
        var match = TickerRegex().Match(produto.Trim());
        return match.Success ? match.Value.ToUpperInvariant() : null;
    }

    [GeneratedRegex(@"[A-Z][A-Z0-9]{3}\d{1,2}", RegexOptions.IgnoreCase)]
    private static partial Regex TickerRegex();

    private static bool EhLinhaTotal(IXLWorksheet planilha, int linha)
    {
        var ultimaColuna = planilha.Row(linha).LastCellUsed()?.Address.ColumnNumber ?? 0;

        for (var col = 1; col <= ultimaColuna; col++)
        {
            var valor = planilha.Cell(linha, col).GetString().Trim();
            if (valor.Equals("Total", StringComparison.OrdinalIgnoreCase))
                return true;
        }

        return false;
    }

    private static string? LerTexto(
        IXLWorksheet planilha,
        int linha,
        Dictionary<string, int> colunas,
        string chave)
    {
        if (!colunas.TryGetValue(chave, out var coluna))
            return null;

        var texto = planilha.Cell(linha, coluna).GetString().Trim();
        return string.IsNullOrEmpty(texto) ? null : texto;
    }

    private static DateTime? LerData(
        IXLWorksheet planilha,
        int linha,
        Dictionary<string, int> colunas,
        string chave)
    {
        if (!colunas.TryGetValue(chave, out var coluna))
            return null;

        var celula = planilha.Cell(linha, coluna);
        if (celula.TryGetValue(out DateTime data))
            return data;

        var texto = celula.GetString().Trim();
        if (string.IsNullOrEmpty(texto))
            return null;

        string[] formatos = ["dd/MM/yyyy", "d/M/yyyy", "yyyy-MM-dd"];
        return DateTime.TryParseExact(texto, formatos, CultureInfo.GetCultureInfo("pt-BR"),
            DateTimeStyles.None, out var valor)
            ? valor
            : null;
    }

    private static decimal? LerDecimal(
        IXLWorksheet planilha,
        int linha,
        Dictionary<string, int> colunas,
        string chave)
    {
        if (!colunas.TryGetValue(chave, out var coluna))
            return null;

        var celula = planilha.Cell(linha, coluna);

        if (celula.TryGetValue(out double numero))
            return Convert.ToDecimal(numero);

        var texto = celula.GetString().Trim();
        if (string.IsNullOrEmpty(texto) || texto == "-")
            return null;

        texto = texto
            .Replace("R$", "", StringComparison.OrdinalIgnoreCase)
            .Replace(".", "")
            .Replace(',', '.')
            .Trim();

        return decimal.TryParse(texto, NumberStyles.Any, CultureInfo.InvariantCulture, out var valor)
            ? valor
            : null;
    }

    private static string Normalizar(string texto)
    {
        var semAcentos = texto.Normalize(NormalizationForm.FormD);
        var builder = new StringBuilder(semAcentos.Length);

        foreach (var caractere in semAcentos)
        {
            if (CharUnicodeInfo.GetUnicodeCategory(caractere) != UnicodeCategory.NonSpacingMark)
                builder.Append(caractere);
        }

        return builder.ToString().Normalize(NormalizationForm.FormC);
    }
}
