# Valida builds Docker da API e do frontend (usados no deploy)
$ErrorActionPreference = "Stop"
$raiz = Split-Path -Parent (Split-Path -Parent $MyInvocation.MyCommand.Path)

Write-Host "==> Build API (backend/API/Dockerfile)" -ForegroundColor Cyan
docker build -f "$raiz/backend/API/Dockerfile" -t carteirapro-api:build-test "$raiz/backend"
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }

Write-Host "==> Build Frontend production (VITE_API_URL de exemplo)" -ForegroundColor Cyan
docker build `
  -f "$raiz/frontend/Dockerfile" `
  --target production `
  --build-arg VITE_API_URL=https://api.example.com/api `
  -t carteirapro-frontend:build-test `
  "$raiz/frontend"
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }

Write-Host ""
Write-Host "OK: imagens carteirapro-api:build-test e carteirapro-frontend:build-test" -ForegroundColor Green
Write-Host "Proximo passo: siga docs/DEPLOY-FREE.md"
