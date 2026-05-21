# Deploy gratuito (Neon + Render + Vercel)

Guia para publicar o CarteiraPro com URL pública usando apenas planos gratuitos, sem cartão de crédito.

## Stack

| Camada | Provedor | Plano | Limite relevante |
|---|---|---|---|
| Postgres | [Neon](https://neon.tech) | Free | 0,5 GB, sem expiração |
| API .NET | [Render](https://render.com) | Free | 750h/mês, dorme após 15min ociosa |
| Frontend | [Vercel](https://vercel.com) | Hobby | Sempre ativo, CDN global |

```mermaid
flowchart LR
  user[Usuario] --> vercel[Frontend Vercel]
  vercel --> render[API Render]
  render --> neon[Postgres Neon]
  render --> brapi[Brapi]
```

**Trade-off principal:** a API no Render Free **dorme** após 15 minutos sem requisição. A primeira chamada depois do "cochilo" demora ~30s para acordar. Para ambiente de testes pessoais é tolerável.

## 1. Postgres no Neon

1. Crie conta em [console.neon.tech](https://console.neon.tech) (login com GitHub).
2. **Create Project** → nome `carteirapro`, região mais próxima.
3. Em **Connection Details**, copie a string em formato `postgresql://...` (vamos converter no passo da API).

## 2. API no Render

1. Crie conta em [render.com](https://render.com) (login com GitHub).
2. **New +** → **Blueprint** → conecte o repositório → o Render detecta o [`render.yaml`](../render.yaml) e cria o serviço `carteirapro-api`.
3. Após o primeiro build, abra o serviço → **Environment** e preencha as variáveis marcadas como `sync: false`:

| Variável | Valor |
|---|---|
| `ConnectionStrings__DefaultConnection` | Ver abaixo |
| `Jwt__Key` | String aleatória de pelo menos 32 caracteres |
| `Brapi__Token` | Seu token da [Brapi](https://brapi.dev) |
| `Cors__Origins__0` | URL pública do Vercel (preenchido depois do passo 3) |

### Connection string Neon → formato .NET/Npgsql

Neon entrega `postgresql://user:pass@host/db?sslmode=require`. O .NET (Npgsql) espera o formato chave=valor. Converta para:

```
Host=ep-xxx-xxx.us-east-2.aws.neon.tech;Database=carteirapro;Username=neondb_owner;Password=COLAR_SENHA;Ssl Mode=Require;Trust Server Certificate=true
```

> Substitua `Host`, `Username` e `Password` pelos valores reais. `Trust Server Certificate=true` evita erro de cadeia de certificados.

4. Aguarde o redeploy automático. O healthcheck do `/health` indica sucesso.
5. Copie a URL gerada (algo como `https://carteirapro-api.onrender.com`).

> Não criar pelo blueprint? Alternativa manual: **New Web Service** → conectar repo → Runtime **Docker** → Root Directory `backend` → Dockerfile Path `API/Dockerfile`. Repita as variáveis acima.

## 3. Frontend no Vercel

1. Crie conta em [vercel.com](https://vercel.com).
2. **Add New** → **Project** → importar o mesmo repositório.
3. Em **Configure Project**:
   - **Root Directory**: `frontend`
   - **Framework Preset**: Vite (auto)
   - **Build/Output**: já vêm de [`frontend/vercel.json`](../frontend/vercel.json)
4. Em **Environment Variables**, adicione:

| Variável | Valor |
|---|---|
| `VITE_API_URL` | `https://carteirapro-api.onrender.com/api` (URL do passo 2 + `/api`) |

5. **Deploy**.
6. Anote a URL pública (ex.: `https://carteirapro.vercel.app`).

## 4. Liberar CORS na API

Volte ao serviço Render → **Environment** → `Cors__Origins__0` = URL do Vercel (com `https://`, sem barra final).

Salvar dispara redeploy. Aguarde voltar healthy.

## 5. Validação

| Teste | Como |
|---|---|
| API acordada | `curl https://carteirapro-api.onrender.com/health` → `{"status":"ok"}` |
| Swagger | abrir `https://<api>/swagger` |
| App | abrir URL do Vercel |
| Cadastro/login | criar conta e entrar |
| Importação B3 | importar `.xlsx` com preço médio preenchido |

## 6. Manutenção

### Acordar a API

Para evitar o cold start de 30s ao acessar o app:

- Use [UptimeRobot](https://uptimerobot.com) (free) e crie um monitor HTTP para `https://<api>/health` a cada 14 minutos. A API fica viva.
- Limite Render Free: 750h/mês ≈ 31 dias. Manter sempre acordado consome a cota; em testes esporádicos, deixe dormir.

### Atualizar a aplicação

`git push` na branch conectada → Render e Vercel rebuildam automaticamente.

### Migrations

`Database__AutoMigrate=true` está no [`render.yaml`](../render.yaml). Toda subida da API aplica migrations pendentes no Neon.

## Troubleshooting

| Sintoma | Causa provável |
|---|---|
| 502/503 ao abrir app | API dormindo — aguarde 30s e recarregue |
| CORS no console | `Cors__Origins__0` sem URL exata do Vercel |
| `28P01: senha falhou` | senha errada na connection string Neon |
| `SSL connection required` | falta `Ssl Mode=Require` na string |
| Frontend chama `/api` mas falha 404 | `VITE_API_URL` ausente — redeploy do Vercel |

## Outras alternativas

- **Oracle Cloud Free** (VM grátis pra sempre): instalar Docker e rodar [`docker-compose.yml`](../docker-compose.yml) atrás de Caddy/Traefik para HTTPS.
- **Railway / Fly.io** (pagos): mais simples, sem cold start, ~$5/mês após o crédito inicial.
