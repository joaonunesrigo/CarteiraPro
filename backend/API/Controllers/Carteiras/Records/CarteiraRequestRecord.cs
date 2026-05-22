using Domain.Enums;

namespace API.Controllers.Carteiras.Records;

public record CarteiraRequestRecord(string Nome, string? Descricao, Moeda Moeda, bool Padrao = false);
