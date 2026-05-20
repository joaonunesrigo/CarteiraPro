using System.Text.RegularExpressions;
using Application.Exceptions;
using ClosedXML.Excel;
using Domain.Enums;
using Domain.Interfaces;
using Domain.Models;

namespace Infrastructure.Importacao;

public partial class B3PosicaoParser : IB3PosicaoParser
{
    private static readonly (string[] Nomes, TipoAtivo TipoPadrao, bool TickerNoProduto)[] ConfigAbas =
    [
        (["Acoes"], TipoAtivo.AcaoBR, false),
        (["Fundo de Investimento"], TipoAtivo.FII, false),
        (["Empréstimos", "Emprestimos"], TipoAtivo.AcaoBR, true),
    ];

    private static readonly Dictionary<string, string> AliasesColunas = new(StringComparer.OrdinalIgnoreCase)
    {
        ["Código de Negociação"] = "ticker",
        ["Codigo de Negociacao"] = "ticker",
        ["Quantidade Disponível"] = "quantidade",
        ["Quantidade Disponivel"] = "quantidade",
        ["Quantidade"] = "quantidade",
        ["Preço de Fechamento"] = "precoFechamento",
        ["Preco de Fechamento"] = "precoFechamento",
        ["Produto"] = "produto",
    };

    public IReadOnlyList<LinhaPosicaoB3> Parse(Stream arquivo)
    {
        using var workbook = new XLWorkbook(arquivo);
        var linhas = new List<LinhaPosicaoB3>();

        foreach (var (nomes, tipoPadrao, tickerNoProduto) in ConfigAbas)
        {
            var planilha = ObterPlanilha(workbook, nomes);
            if (planilha is null)
                continue;

            var nomeAba = planilha.Name;
            linhas.AddRange(LerAba(planilha, nomeAba, tipoPadrao, tickerNoProduto));
        }

        if (linhas.Count == 0)
            throw new ArquivoB3InvalidoException(
                "Nenhum ativo encontrado. Use o Excel de posição da B3 (abas Acoes, Fundo de Investimento e/ou Empréstimos).");

        return linhas
            .GroupBy(l => l.Ticker, StringComparer.OrdinalIgnoreCase)
            .Select(ConsolidarGrupo)
            .ToList();
    }

    private static IXLWorksheet? ObterPlanilha(XLWorkbook workbook, string[] nomes)
    {
        foreach (var planilha in workbook.Worksheets)
        {
            foreach (var nome in nomes)
            {
                if (planilha.Name.Equals(nome, StringComparison.OrdinalIgnoreCase))
                    return planilha;
            }
        }

        return null;
    }

    private static LinhaPosicaoB3 ConsolidarGrupo(IEnumerable<LinhaPosicaoB3> grupo)
    {
        var lista = grupo.ToList();
        if (lista.Count == 1)
            return lista[0];

        var custodia = lista.FirstOrDefault(l =>
            l.OrigemAba.Equals("Acoes", StringComparison.OrdinalIgnoreCase)
            || l.OrigemAba.Equals("Fundo de Investimento", StringComparison.OrdinalIgnoreCase));

        if (custodia is not null)
            return custodia;

        return lista.OrderByDescending(l => l.Quantidade).First();
    }

    private static IEnumerable<LinhaPosicaoB3> LerAba(
        IXLWorksheet planilha,
        string nomeAba,
        TipoAtivo tipoPadrao,
        bool tickerNoProduto)
    {
        var ultimaLinha = planilha.LastRowUsed()?.RowNumber() ?? 0;
        if (ultimaLinha == 0)
            yield break;

        var (linhaCabecalho, colunas) = EncontrarCabecalho(planilha, ultimaLinha, tickerNoProduto);
        if (linhaCabecalho == 0)
            yield break;

        for (var linha = linhaCabecalho + 1; linha <= ultimaLinha; linha++)
        {
            var produto = LerTexto(planilha, linha, colunas, "produto");
            var ticker = LerTexto(planilha, linha, colunas, "ticker");

            if (string.IsNullOrWhiteSpace(ticker) && !string.IsNullOrWhiteSpace(produto))
                ticker = ExtrairTickerDoProduto(produto);

            if (string.IsNullOrWhiteSpace(ticker))
                continue;

            if (EhLinhaTotal(planilha, linha))
                continue;

            var quantidade = LerDecimal(planilha, linha, colunas, "quantidade");
            if (quantidade is null or <= 0)
                continue;

            var precoFechamento = LerDecimal(planilha, linha, colunas, "precoFechamento");
            if (precoFechamento is 0)
                precoFechamento = null;

            var tipo = InferirTipo(ticker, tipoPadrao);

            yield return new LinhaPosicaoB3
            {
                Ticker = ticker.ToUpperInvariant(),
                Produto = produto,
                Quantidade = quantidade.Value,
                PrecoFechamento = precoFechamento,
                Tipo = tipo,
                OrigemAba = nomeAba,
            };
        }
    }

    private static (int LinhaCabecalho, Dictionary<string, int> Colunas) EncontrarCabecalho(
        IXLWorksheet planilha,
        int ultimaLinha,
        bool tickerNoProduto)
    {
        var limite = Math.Min(ultimaLinha, 15);

        for (var linha = 1; linha <= limite; linha++)
        {
            var colunas = new Dictionary<string, int>();
            var ultimaColuna = planilha.Row(linha).LastCellUsed()?.Address.ColumnNumber ?? 0;

            for (var col = 1; col <= ultimaColuna; col++)
            {
                var titulo = planilha.Cell(linha, col).GetString().Trim();
                if (string.IsNullOrEmpty(titulo))
                    continue;

                if (AliasesColunas.TryGetValue(titulo, out var chave))
                    colunas[chave] = col;
            }

            var cabecalhoValido = colunas.ContainsKey("ticker")
                || (tickerNoProduto && colunas.ContainsKey("produto") && colunas.ContainsKey("quantidade"));

            if (cabecalhoValido)
                return (linha, colunas);
        }

        return (0, new Dictionary<string, int>());
    }

    private static string? ExtrairTickerDoProduto(string produto)
    {
        var texto = produto.Trim();
        if (string.IsNullOrEmpty(texto))
            return null;

        var antesDoHifen = texto.Split(" - ", 2, StringSplitOptions.TrimEntries)[0];
        var candidato = antesDoHifen.Split(' ', StringSplitOptions.RemoveEmptyEntries)[0];

        return TickerRegex().IsMatch(candidato) ? candidato.ToUpperInvariant() : null;
    }

    [GeneratedRegex(@"^[A-Z][A-Z0-9]{3}\d{1,2}$", RegexOptions.IgnoreCase)]
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

        texto = texto.Replace(".", "").Replace(',', '.');
        return decimal.TryParse(texto, System.Globalization.NumberStyles.Any,
            System.Globalization.CultureInfo.InvariantCulture, out var valor)
            ? valor
            : null;
    }

    private static TipoAtivo InferirTipo(string ticker, TipoAtivo tipoPadrao)
    {
        if (tipoPadrao == TipoAtivo.FII)
            return TipoAtivo.FII;

        if (ticker.EndsWith("11", StringComparison.OrdinalIgnoreCase))
            return TipoAtivo.FII;

        return TipoAtivo.AcaoBR;
    }
}
