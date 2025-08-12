param (
    [string]$buildPath = "..\Build\Debug\net8.0-windows7.0"
)

# 전체 경로를 절대경로로 변환
$fullPath = Resolve-Path $buildPath

# 로그
Write-Host "Cleaning logs and saves in: $fullPath"

# 삭제 대상 폴더 목록
$foldersToDelete = @("Logs")

foreach ($folder in $foldersToDelete) {
    $targetPath = Join-Path $fullPath $folder

    if (Test-Path $targetPath) {
        Write-Host "Removing folder: $targetPath"
        Remove-Item -Path $targetPath -Recurse -Force
    } else {
        Write-Host "Folder not found: $targetPath"
    }
}

Write-Host "Cleanup complete."