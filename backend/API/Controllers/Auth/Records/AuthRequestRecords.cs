namespace API.Controllers.Auth.Records;

public record RegistrarRequestRecord(string Nome, string Email, string Senha);

public record LoginRequestRecord(string Email, string Senha);
