# Limpa cache do Visual Studio Container Tools (use se o erro de copiar API.dll persistir)
$root = Split-Path -Parent $PSScriptRoot

Write-Host "Removendo cache Docker do Visual Studio..."
Remove-Item -Recurse -Force "$root\obj\Docker" -ErrorAction SilentlyContinue
Remove-Item -Recurse -Force "$root\backend\API\obj" -ErrorAction SilentlyContinue
Remove-Item -Recurse -Force "$root\backend\API\bin" -ErrorAction SilentlyContinue

Write-Host "Concluído. Feche o Visual Studio, execute este script e abra a solução novamente."
