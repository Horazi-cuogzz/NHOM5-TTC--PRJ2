param(
    [switch]$InitializeDatabase,
    [int]$Port = 5055
)
$ErrorActionPreference = 'Stop'
$projectRoot = Join-Path $PSScriptRoot 'RecruitHub.Web'
$projectFile = Join-Path $projectRoot 'N5RecruitHub.Web.csproj'
$previousEnvironment = $env:ASPNETCORE_ENVIRONMENT
$previousInitialize = $env:Database__Initialize
try {
    Push-Location (Join-Path $projectRoot 'ClientApp')
    try {
        if (-not (Test-Path -LiteralPath 'node_modules')) {
            npm ci --no-fund --no-audit
            if ($LASTEXITCODE -ne 0) { throw 'Khong cai duoc thu vien frontend.' }
        }
        npm run build
        if ($LASTEXITCODE -ne 0) { throw 'Khong build duoc dashboard React.' }
    } finally { Pop-Location }
    $env:ASPNETCORE_ENVIRONMENT = 'Development'
    $env:Database__Initialize = if ($InitializeDatabase) { 'true' } else { 'false' }
    Write-Host "RecruitHub: http://localhost:$Port"
    dotnet run --project $projectFile --no-launch-profile --urls "http://localhost:$Port"
    if ($LASTEXITCODE -ne 0) { throw 'Ung dung chua khoi chay thanh cong. Xem thong bao phia tren.' }
} finally {
    $env:ASPNETCORE_ENVIRONMENT = $previousEnvironment
    $env:Database__Initialize = $previousInitialize
}

