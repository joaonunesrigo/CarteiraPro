# CarteiraPro

Aplicação de gestão de carteira de investimentos (backend .NET, frontend React/Vite, PostgreSQL).

## Docker (recomendado)

### Pré-requisitos

- [Docker Desktop](https://www.docker.com/products/docker-desktop/) em execução
- Token da [brapi.dev](https://brapi.dev) para cotações

### Desenvolvimento

```bash
# O arquivo .env já existe na raiz — cole seu token em BRAPI_TOKEN antes de subir
docker compose up --build
```

| Serviço    | URL                          |
|------------|------------------------------|
| Frontend   | http://localhost:5173        |
| API        | http://localhost:5122        |
| Swagger    | http://localhost:5122/swagger |
| PostgreSQL | localhost:5432               |

O stack sobe PostgreSQL, aplica migrações do EF Core automaticamente e inicia API + frontend com hot reload.

### Produção (build estático + nginx)

```bash
docker compose -f docker-compose.yml -f docker-compose.prod.yml up --build -d
```

| Serviço  | URL                   |
|----------|-----------------------|
| Frontend | http://localhost      |
| API      | http://localhost:5122 |

### Comandos úteis

```bash
docker compose down          # para os containers
docker compose down -v       # remove volumes (apaga dados do Postgres)
docker compose logs -f api   # logs da API
```

### Problemas comuns

**Postgres 18 não inicia / erro ao montar volume**

A imagem oficial do Postgres 18 exige montar o volume em `/var/lib/postgresql` (não em `/var/lib/postgresql/data`). Se você atualizou da versão 16, recrie o volume:

```bash
docker compose down -v
docker compose up --build
```

**`dockerDesktopLinuxEngine` não encontrado**

Inicie o Docker Desktop e aguarde ficar pronto antes de rodar `docker compose`.
