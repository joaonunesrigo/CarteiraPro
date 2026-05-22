using Domain.Enums;

namespace Application.Services.Carteiras.DTOs;

public record CarteiraDto(
    Guid Id,
    string Nome,
    string? Descricao,
    Moeda Moeda,
    bool Padrao,
    DateTime DataCadastro);
