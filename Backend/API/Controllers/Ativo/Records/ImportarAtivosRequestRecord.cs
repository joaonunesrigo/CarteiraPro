using Application.Dtos;

namespace API.Controllers.Ativo.Records;

public record ImportarAtivosRequestRecord(
    IReadOnlyList<ImportarAtivoItemDto> Ativos,
    bool IgnorarDuplicados = true);
