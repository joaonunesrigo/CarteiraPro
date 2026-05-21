# CarteiraPro

MVP para acompanhamento de carteira de investimentos no Brasil, com backend em .NET, frontend em React e banco PostgreSQL.

## O que o MVP entrega

- Carteira com cadastro manual de ativos e importacao de posicao da B3.
- Historico de operacoes de compra/venda como fonte de verdade para quantidade e preco medio.
- Rentabilidade com cotacao atual via Brapi, cache no backend e refresh automatico.
- Grafico de desempenho da carteira, alocacao por ativo/tipo e rentabilidade por ativo.
- Proventos com CRUD, importacao B3, resumo e grafico mensal.
- UI com tabelas paginadas, modais, toasts e confirmacoes.

## Stack

- Backend: .NET 10, ASP.NET Core, Entity Framework Core, PostgreSQL.
- Frontend: React, Vite, React Query, Zustand, Recharts e Tailwind CSS.
- Infra local: Docker Compose.

## Pré-requisitos

- [Docker Desktop](https://www.docker.com/products/docker-desktop/) em execução.
- Token da [Brapi](https://brapi.dev) para cotacoes.

## Como rodar

1. Crie o arquivo `.env` a partir do exemplo:

```bash
cp .env.example .env
```

No Windows PowerShell:

```powershell
Copy-Item .env.example .env
```

2. Edite `.env` e defina `BRAPI_TOKEN`.

3. Suba os containers:

```bash
docker compose up --build
```

## URLs locais

| Serviço | URL |
| --- | --- |
| Frontend | http://localhost:5173 |
| API / Swagger | http://localhost:5122/swagger |
| Health check | http://localhost:5122/health |

## Comandos úteis

```bash
# Parar os containers
docker compose down

# Subir novamente sem rebuild
docker compose up -d

# Rebuild apenas da API
docker compose build api
docker compose up -d api

# Logs da API
docker compose logs -f api

# Logs do frontend
docker compose logs -f frontend
```

## Desenvolvimento sem Docker

Backend:

```bash
cd backend
dotnet build API/API.csproj
dotnet run --project API/API.csproj
```

Frontend:

```bash
cd frontend
npm install
npm run dev
```

## Variáveis de ambiente

| Variável | Descrição | Padrão local |
| --- | --- | --- |
| `POSTGRES_PASSWORD` | Senha do usuário `postgres` no container | `postgres` |
| `BRAPI_TOKEN` | Token usado para consultar cotações na Brapi | vazio |
| `JWT_KEY` | Chave para assinar tokens JWT | ver `.env.example` |

## Deploy de testes

Stack gratuito: Neon (Postgres) + Render (API) + Vercel (frontend) — 100% grátis, sem cartão.

Guia passo a passo: **[docs/DEPLOY-FREE.md](docs/DEPLOY-FREE.md)**. Blueprint pronto em [`render.yaml`](render.yaml) e config Vercel em [`frontend/vercel.json`](frontend/vercel.json).

Validação local dos builds Docker:

```powershell
.\scripts\validate-build.ps1
```

## Observações

- As migrations são aplicadas automaticamente no container da API (`Database__AutoMigrate=true` no `docker-compose.yml`).
- A importacao de posicao da B3 representa a carteira a partir da data de importacao; historico anterior exige operacoes cadastradas com as datas reais.
- Artefatos gerados (`bin/`, `obj/`, `dist/`, `node_modules/`) ficam fora do Git pelo `.gitignore`.
