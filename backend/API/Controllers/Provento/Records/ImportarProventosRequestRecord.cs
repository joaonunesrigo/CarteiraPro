using Application.Dtos;

namespace API.Controllers.Provento.Records;

public record ImportarProventosRequestRecord(
    IReadOnlyList<ImportarProventoItemDto> Proventos,
    bool IgnorarDuplicados = true);
