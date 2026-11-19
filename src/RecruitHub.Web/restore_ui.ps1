# Script khoi phuc giao dien ve nguyen ban truoc khi them animation
$root = Split-Path -Parent $MyInvocation.MyCommand.Definition
$backupDir = Join-Path $root "backup_ui_original"

if (Test-Path $backupDir) {
    Copy-Item (Join-Path $backupDir "n5-site.css") (Join-Path $root "wwwroot\css\n5-site.css") -Force
    Copy-Item (Join-Path $backupDir "n5-site.js") (Join-Path $root "wwwroot\js\n5-site.js") -Force
    Copy-Item (Join-Path $backupDir "N5Dashboard.jsx") (Join-Path $root "ClientApp\N5Dashboard.jsx") -Force
    Write-Host "[OK] Da khoi phuc nguyen ban toan bo giao dien va CSS/JS thanh cong!" -ForegroundColor Green
} else {
    Write-Host "[ERROR] Khong tim thay thu muc backup_ui_original!" -ForegroundColor Red
}
