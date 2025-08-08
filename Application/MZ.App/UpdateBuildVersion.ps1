
param (
    $jsonPath = "appsettings.json"
)

# 현재 시간
$timestamp = Get-Date -Format "yy.MM.dd.HH.mm.ss.ff"

# JSON Load
$json = Get-Content $jsonPath -Raw | ConvertFrom-Json

# 버전 삽입
$json.Build.Version = $timestamp

# 저장
$json | ConvertTo-Json -Depth 10 | Set-Content $jsonPath -Encoding UTF8

Write-Host "Updated Build Version to $timestamp"