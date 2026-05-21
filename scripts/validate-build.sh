#!/usr/bin/env bash
set -euo pipefail
ROOT="$(cd "$(dirname "$0")/.." && pwd)"

echo "==> Build API (backend/API/Dockerfile)"
docker build -f "$ROOT/backend/API/Dockerfile" -t carteirapro-api:build-test "$ROOT/backend"

echo "==> Build Frontend production (VITE_API_URL de exemplo)"
docker build \
  -f "$ROOT/frontend/Dockerfile" \
  --target production \
  --build-arg VITE_API_URL=https://api.example.com/api \
  -t carteirapro-frontend:build-test \
  "$ROOT/frontend"

echo ""
echo "OK: imagens carteirapro-api:build-test e carteirapro-frontend:build-test"
echo "Proximo passo: siga docs/DEPLOY-FREE.md"
