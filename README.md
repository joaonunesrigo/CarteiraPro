# CarteiraPro

Gestão de carteira de investimentos (.NET, React, PostgreSQL).

## Pré-requisitos

- [Docker Desktop](https://www.docker.com/products/docker-desktop/) em execução
- Token da [brapi.dev](https://brapi.dev)

## Como rodar

```bash
cp .env.example .env
# Edite .env e defina BRAPI_TOKEN

docker compose up --build
```

| Serviço  | URL |
|----------|-----|
| Frontend | http://localhost:5173 |
| API / Swagger | http://localhost:5122/swagger |

Para parar: `docker compose down`
