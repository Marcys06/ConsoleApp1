# ============================================
# UpdateVersion.ps1 – generuje wersję z typem zmiany
# Użycie: .\UpdateVersion.ps1 [typ]
#   typ: M (major), m (minor), p (patch), b (beta)
# ============================================

param(
    [Parameter(Mandatory=$true)]
    [ValidateSet('M', 'm', 'p', 'b')]
    [string]$Type
)

# ===== GENERUJ NUMER WERSJI =====
$date = Get-Date -Format "yyyy.MM.dd"
$time = Get-Date -Format "HHmm"
$version = "$date.$time-$Type"

# ===== MAPOWANIE TYPÓW =====
$typeNames = @{
    'M' = 'MAJOR'
    'm' = 'MINOR'
    'p' = 'PATCH'
    'b' = 'BETA'
}
$typeName = $typeNames[$Type]

# ===== AKTUALIZUJ CHANGELOG =====
$changelogPath = "CHANGELOG.md"
$entry = @"
## [$version] - $(Get-Date -Format "yyyy-MM-dd")

### 🏷️ TYP ZMIANY: $typeName

### 📝 ZMIANY
- 

### 🐛 POPRAWIONE
- 

---
"@

# Dodaj wpis do CHANGELOG
if (Test-Path $changelogPath) {
    $content = Get-Content -Path $changelogPath -Raw
    $headerEnd = $content.IndexOf("## [")
    if ($headerEnd -eq -1) {
        $newContent = $content + $entry
    } else {
        $newContent = $content.Substring(0, $headerEnd) + $entry + $content.Substring($headerEnd)
    }
    Set-Content -Path $changelogPath -Value $newContent
} else {
    Set-Content -Path $changelogPath -Value "# CHANGELOG`n`n$entry"
}

Write-Host "✅ Dodano wpis w CHANGELOG dla wersji $version ($typeName)"

# ===== AKTUALIZUJ WERSJĘ W .csproj =====
$csprojPath = "TTD.Main/TTD.Main.csproj"
if (Test-Path $csprojPath) {
    $content = Get-Content -Path $csprojPath -Raw
    $content = $content -replace '<Version>.*?</Version>', "<Version>$version</Version>"
    $content = $content -replace '<FileVersion>.*?</FileVersion>', "<FileVersion>$version</FileVersion>"
    Set-Content -Path $csprojPath -Value $content
    Write-Host "✅ Zaktualizowano wersję w .csproj na $version"
}

# ===== EKSTRA – DODAJ TAG GIT =====
$tag = "v$version"
Write-Host ""
Write-Host "📋 Aby dodać tag Git, wykonaj:"
Write-Host "   git add ."
Write-Host "   git commit -m `"$tag`""
Write-Host "   git tag $tag"
Write-Host "   git push --tags"