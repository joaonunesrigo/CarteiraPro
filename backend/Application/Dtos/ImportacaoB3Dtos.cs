using Domain.Enums;

namespace Application.Dtos;

public record LinhaImportacaoB3Dto(
    string Ticker,
    string? Produto,
    decimal Quantidade,
    decimal? PrecoFechamento,
    decimal? PrecoMedio,
    TipoAtivo Tipo,
    bool JaCadastrado,
    string OrigemAba);

public record ImportarAtivoItemDto(
    string Ticker,
    decimal PrecoMedio,
    decimal Quantidade,
    TipoAtivo Tipo);

public record ImportarAtivosResultadoDto(
    int Importados,
    int Ignorados,
    IReadOnlyList<ErroImportacaoDto> Erros);

public record ErroImportacaoDto(string Ticker, string Mensagem);
