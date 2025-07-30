function Get-FolderTree {
    param (
        [string]$Path = (Get-Location),
        [int]$Depth = 0,
        [string]$Prefix = ""
    )

    $folders = Get-ChildItem -Path $Path -Directory | Sort-Object Name

    for ($i = 0; $i -lt $folders.Count; $i++) {
        $folder = $folders[$i]
        $isLast = ($i -eq ($folders.Count - 1))

        $currentPrefix = if ($isLast) {
            $Prefix + "└── "
        } else {
            $Prefix + "├── "
        }

        Write-Host "$currentPrefix$($folder.Name)"

        $newPrefix = if ($isLast) {
            $Prefix + "    "
        } else {
            $Prefix + "│   "
        }
        Get-FolderTree -Path $folder.FullName -Depth ($Depth + 1) -Prefix $newPrefix
    }
}

# 현재 디렉토리의 폴더 구조를 출력합니다.
Get-FolderTree