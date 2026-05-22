namespace Application.Catalogs;

/// <summary>
/// Fallback de setor para ações brasileiras quando a Brapi não traz
/// <c>summaryProfile.sector</c>. Cobre as principais empresas listadas
/// na B3. Tickers não listados continuam usando o valor da Brapi (que
/// para ações funciona bem na maioria dos casos).
/// </summary>
public static class SetoresAcoesCatalog
{
    private static readonly Dictionary<string, string> Mapa = new(StringComparer.OrdinalIgnoreCase)
    {
        // ===== Bancos =====
        ["BBAS3"] = "Bancos",
        ["ITUB3"] = "Bancos",
        ["ITUB4"] = "Bancos",
        ["ITSA3"] = "Bancos",
        ["ITSA4"] = "Bancos",
        ["BBDC3"] = "Bancos",
        ["BBDC4"] = "Bancos",
        ["SANB3"] = "Bancos",
        ["SANB4"] = "Bancos",
        ["SANB11"] = "Bancos",
        ["BPAC3"] = "Bancos",
        ["BPAC5"] = "Bancos",
        ["BPAC11"] = "Bancos",
        ["BMGB4"] = "Bancos",
        ["BRSR3"] = "Bancos",
        ["BRSR6"] = "Bancos",
        ["BAZA3"] = "Bancos",
        ["BNBR3"] = "Bancos",
        ["INBR32"] = "Bancos",
        ["ABCB4"] = "Bancos",
        ["BPAN4"] = "Bancos",

        // ===== Seguros =====
        ["BBSE3"] = "Seguros",
        ["IRBR3"] = "Seguros",
        ["PSSA3"] = "Seguros",
        ["SULA11"] = "Seguros",
        ["CXSE3"] = "Seguros",
        ["WIZS3"] = "Seguros",

        // ===== Bolsa e Serviços Financeiros =====
        ["B3SA3"] = "Serviços Financeiros",
        ["CIEL3"] = "Serviços Financeiros",
        ["STNE3"] = "Serviços Financeiros",
        ["PAGS3"] = "Serviços Financeiros",
        ["XPBR31"] = "Serviços Financeiros",

        // ===== Energia (Petróleo, Gás e Biocombustíveis) =====
        ["PETR3"] = "Energia",
        ["PETR4"] = "Energia",
        ["PRIO3"] = "Energia",
        ["RECV3"] = "Energia",
        ["RRRP3"] = "Energia",
        ["ENAT3"] = "Energia",
        ["VBBR3"] = "Energia",
        ["UGPA3"] = "Energia",
        ["CSAN3"] = "Energia",
        ["RAIZ4"] = "Energia",

        // ===== Energia Elétrica =====
        ["EGIE3"] = "Energia Elétrica",
        ["CMIG3"] = "Energia Elétrica",
        ["CMIG4"] = "Energia Elétrica",
        ["CPFE3"] = "Energia Elétrica",
        ["ELET3"] = "Energia Elétrica",
        ["ELET6"] = "Energia Elétrica",
        ["NEOE3"] = "Energia Elétrica",
        ["ENBR3"] = "Energia Elétrica",
        ["ENGI11"] = "Energia Elétrica",
        ["TAEE3"] = "Energia Elétrica",
        ["TAEE4"] = "Energia Elétrica",
        ["TAEE11"] = "Energia Elétrica",
        ["AESB3"] = "Energia Elétrica",
        ["EQTL3"] = "Energia Elétrica",
        ["CPLE3"] = "Energia Elétrica",
        ["CPLE6"] = "Energia Elétrica",
        ["AURE3"] = "Energia Elétrica",
        ["TRPL3"] = "Energia Elétrica",
        ["TRPL4"] = "Energia Elétrica",
        ["ENEV3"] = "Energia Elétrica",
        ["LIGT3"] = "Energia Elétrica",
        ["ALUP11"] = "Energia Elétrica",
        ["ALUP3"] = "Energia Elétrica",
        ["ALUP4"] = "Energia Elétrica",
        ["AXIA6"] = "Energia Elétrica",
        ["AXIA7"] = "Energia Elétrica",

        // ===== Saneamento =====
        ["SBSP3"] = "Saneamento",
        ["CSMG3"] = "Saneamento",
        ["SAPR3"] = "Saneamento",
        ["SAPR4"] = "Saneamento",
        ["SAPR11"] = "Saneamento",
        ["CASN4"] = "Saneamento",
        ["AMBP3"] = "Saneamento",
        ["ORVR3"] = "Saneamento",

        // ===== Telecomunicações =====
        ["VIVT3"] = "Telecomunicações",
        ["TIMS3"] = "Telecomunicações",
        ["OIBR3"] = "Telecomunicações",
        ["OIBR4"] = "Telecomunicações",

        // ===== Mineração =====
        ["VALE3"] = "Mineração",
        ["CMIN3"] = "Mineração",
        ["BRAP4"] = "Mineração",
        ["AURA33"] = "Mineração",

        // ===== Siderurgia =====
        ["GGBR3"] = "Siderurgia",
        ["GGBR4"] = "Siderurgia",
        ["GOAU3"] = "Siderurgia",
        ["GOAU4"] = "Siderurgia",
        ["CSNA3"] = "Siderurgia",
        ["USIM3"] = "Siderurgia",
        ["USIM5"] = "Siderurgia",

        // ===== Petroquímico =====
        ["BRKM3"] = "Petroquímico",
        ["BRKM5"] = "Petroquímico",
        ["UNIP6"] = "Petroquímico",

        // ===== Papel e Celulose =====
        ["SUZB3"] = "Papel e Celulose",
        ["KLBN3"] = "Papel e Celulose",
        ["KLBN4"] = "Papel e Celulose",
        ["KLBN11"] = "Papel e Celulose",

        // ===== Tecnologia =====
        ["TOTS3"] = "Tecnologia",
        ["LWSA3"] = "Tecnologia",
        ["CASH3"] = "Tecnologia",
        ["IFCM3"] = "Tecnologia",
        ["POSI3"] = "Tecnologia",
        ["INTB3"] = "Tecnologia",
        ["NINJ3"] = "Tecnologia",

        // ===== Saúde =====
        ["RDOR3"] = "Saúde",
        ["HAPV3"] = "Saúde",
        ["FLRY3"] = "Saúde",
        ["QUAL3"] = "Saúde",
        ["HYPE3"] = "Saúde",
        ["MATD3"] = "Saúde",
        ["ONCO3"] = "Saúde",
        ["BIOM3"] = "Saúde",
        ["DASA3"] = "Saúde",
        ["PNVL3"] = "Saúde",
        ["RADL3"] = "Saúde",
        ["BLAU3"] = "Saúde",

        // ===== Educação =====
        ["COGN3"] = "Educação",
        ["YDUQ3"] = "Educação",
        ["SEER3"] = "Educação",
        ["CSED3"] = "Educação",
        ["ANIM3"] = "Educação",

        // ===== Consumo / Varejo =====
        ["MGLU3"] = "Varejo",
        ["AMER3"] = "Varejo",
        ["VIIA3"] = "Varejo",
        ["LREN3"] = "Varejo",
        ["CEAB3"] = "Varejo",
        ["GUAR3"] = "Varejo",
        ["AMAR3"] = "Varejo",
        ["LJQQ3"] = "Varejo",
        ["SOMA3"] = "Varejo",
        ["ARZZ3"] = "Varejo",
        ["GRND3"] = "Varejo",
        ["ALPA4"] = "Varejo",
        ["VULC3"] = "Varejo",
        ["TFCO4"] = "Varejo",
        ["PETZ3"] = "Varejo",
        ["CRFB3"] = "Varejo",
        ["ASAI3"] = "Varejo",
        ["PCAR3"] = "Varejo",
        ["GMAT3"] = "Varejo",
        ["LJCY3"] = "Varejo",

        // ===== Alimentos e Bebidas =====
        ["ABEV3"] = "Alimentos e Bebidas",
        ["JBSS3"] = "Alimentos e Bebidas",
        ["MRFG3"] = "Alimentos e Bebidas",
        ["BRFS3"] = "Alimentos e Bebidas",
        ["BEEF3"] = "Alimentos e Bebidas",
        ["CAML3"] = "Alimentos e Bebidas",
        ["MDIA3"] = "Alimentos e Bebidas",
        ["SMTO3"] = "Alimentos e Bebidas",
        ["MEAL3"] = "Alimentos e Bebidas",
        ["AGRO3"] = "Alimentos e Bebidas",
        ["SLCE3"] = "Alimentos e Bebidas",
        ["SOJA3"] = "Alimentos e Bebidas",

        // ===== Construção Civil =====
        ["CYRE3"] = "Construção Civil",
        ["MRVE3"] = "Construção Civil",
        ["EZTC3"] = "Construção Civil",
        ["DIRR3"] = "Construção Civil",
        ["EVEN3"] = "Construção Civil",
        ["TEND3"] = "Construção Civil",
        ["MDNE3"] = "Construção Civil",
        ["TRIS3"] = "Construção Civil",
        ["JHSF3"] = "Construção Civil",
        ["LAVV3"] = "Construção Civil",
        ["HBOR3"] = "Construção Civil",
        ["CURY3"] = "Construção Civil",
        ["PLPL3"] = "Construção Civil",

        // ===== Logística e Transporte =====
        ["RAIL3"] = "Logística",
        ["CCRO3"] = "Logística",
        ["AZUL4"] = "Logística",
        ["GOLL4"] = "Logística",
        ["ECOR3"] = "Logística",
        ["JSLG3"] = "Logística",
        ["STBP3"] = "Logística",
        ["HBSA3"] = "Logística",
        ["LOGG3"] = "Logística",
        ["MOVI3"] = "Logística",
        ["RENT3"] = "Logística",
        ["VAMO3"] = "Logística",
        ["SIMH3"] = "Logística",
        ["PORT3"] = "Logística",

        // ===== Bens Industriais =====
        ["WEGE3"] = "Bens Industriais",
        ["EMBR3"] = "Bens Industriais",
        ["TUPY3"] = "Bens Industriais",
        ["POMO3"] = "Bens Industriais",
        ["POMO4"] = "Bens Industriais",
        ["AERI3"] = "Bens Industriais",
        ["KEPL3"] = "Bens Industriais",
        ["ROMI3"] = "Bens Industriais",
        ["FRAS3"] = "Bens Industriais",
        ["RAPT3"] = "Bens Industriais",
        ["RAPT4"] = "Bens Industriais",
        ["MILS3"] = "Bens Industriais",
        ["LEVE3"] = "Bens Industriais",
        ["DXCO3"] = "Bens Industriais",
    };

    public static string? TryObter(string ticker)
    {
        if (string.IsNullOrWhiteSpace(ticker)) return null;
        var chave = ticker.Trim().ToUpperInvariant();
        return Mapa.TryGetValue(chave, out var setor) ? setor : null;
    }
}
