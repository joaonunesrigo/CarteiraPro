namespace Application.Catalogs;

/// <summary>
/// Mapeamento manual de FIIs brasileiros para segmento de atuação.
/// A Brapi classifica quase todos como "Multicategoria"; este catálogo
/// sobrepõe esse valor por algo descritivo. Tickers não listados aqui
/// usam o valor retornado pela Brapi como fallback.
/// </summary>
public static class SegmentosFiisCatalog
{
    private static readonly Dictionary<string, string> Mapa = new(StringComparer.OrdinalIgnoreCase)
    {
        // ===== Logística =====
        ["HGLG11"] = "Logística",
        ["BTLG11"] = "Logística",
        ["XPLG11"] = "Logística",
        ["VILG11"] = "Logística",
        ["ALZR11"] = "Logística",
        ["GGRC11"] = "Logística",
        ["LVBI11"] = "Logística",
        ["RBRL11"] = "Logística",
        ["BRCO11"] = "Logística",
        ["LGCP11"] = "Logística",

        // ===== Shoppings =====
        ["XPML11"] = "Shoppings",
        ["HSML11"] = "Shoppings",
        ["VISC11"] = "Shoppings",
        ["MALL11"] = "Shoppings",
        ["HGBS11"] = "Shoppings",
        ["VSHO11"] = "Shoppings",
        ["FLMA11"] = "Shoppings",

        // ===== Escritórios (lajes corporativas) =====
        ["BRCR11"] = "Escritórios",
        ["HGRE11"] = "Escritórios",
        ["PVBI11"] = "Escritórios",
        ["RCRB11"] = "Escritórios",
        ["JSRE11"] = "Escritórios",
        ["RBRP11"] = "Escritórios",
        ["FAMB11B"] = "Escritórios",
        ["RCRI11"] = "Escritórios",
        ["TEPP11"] = "Escritórios",
        ["BTAL11"] = "Escritórios",

        // ===== Papel (recebíveis / CRI) =====
        ["KNCR11"] = "Papel",
        ["KNSC11"] = "Papel",
        ["KNHY11"] = "Papel",
        ["KNIP11"] = "Papel",
        ["MXRF11"] = "Papel",
        ["HGCR11"] = "Papel",
        ["CPTS11"] = "Papel",
        ["RECR11"] = "Papel",
        ["RBRR11"] = "Papel",
        ["IRDM11"] = "Papel",
        ["VGIP11"] = "Papel",
        ["VGHF11"] = "Papel",
        ["VGIR11"] = "Papel",
        ["BCRI11"] = "Papel",
        ["BTCI11"] = "Papel",
        ["BTCR11"] = "Papel",
        ["OUJP11"] = "Papel",
        ["DEVA11"] = "Papel",
        ["VRTA11"] = "Papel",
        ["URPR11"] = "Papel",

        // ===== Híbrido =====
        ["KNRI11"] = "Híbrido",
        ["RBVA11"] = "Híbrido",
        ["GTWR11"] = "Híbrido",

        // ===== Renda Urbana =====
        ["HGRU11"] = "Renda Urbana",
        ["TRXF11"] = "Renda Urbana",
        ["RBRY11"] = "Renda Urbana",
        ["XPPR11"] = "Renda Urbana",

        // ===== Fundo de Fundos =====
        ["BCFF11"] = "Fundo de Fundos",
        ["KFOF11"] = "Fundo de Fundos",
        ["HGFF11"] = "Fundo de Fundos",
        ["RBFF11"] = "Fundo de Fundos",
        ["BPFF11"] = "Fundo de Fundos",
        ["RFOF11"] = "Fundo de Fundos",
        ["MGFF11"] = "Fundo de Fundos",
        ["ITIP11"] = "Fundo de Fundos",
        ["XPSF11"] = "Fundo de Fundos",

        // ===== Outros (agro, desenvolvimento, saúde, educação, hotelaria, infraestrutura) =====
        ["TGAR11"] = "Outros",
        ["RZAG11"] = "Outros",
        ["DCRA11"] = "Outros",
        ["KISU11"] = "Outros",
        ["HCRI11"] = "Outros",
        ["NSLU11"] = "Outros",
        ["NVHO11"] = "Outros",
        ["RBED11"] = "Outros",
        ["HTMX11"] = "Outros",
        ["BTHF11"] = "Outros",
        ["IFRA11"] = "Outros",
    };

    public static string? TryObter(string ticker)
    {
        if (string.IsNullOrWhiteSpace(ticker)) return null;
        var chave = ticker.Trim().ToUpperInvariant();
        return Mapa.TryGetValue(chave, out var segmento) ? segmento : null;
    }
}
