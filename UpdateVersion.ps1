# ============================================
# UpdateVersion.ps1
#
# System wersjonowania:
#   YYYY.MM.DD-HHMM-MAJOR.MINOR.PATCH
#
# Typy:
#   m = MAJOR
#   n = MINOR
#   p = PATCH
#   f = FIX
#   a = ALPHA
#   b = BETA
#
# Użycie:
#   .\UpdateVersion.ps1 m
#   .\UpdateVersion.ps1 n
#   .\UpdateVersion.ps1 p
#   .\UpdateVersion.ps1 f
#   .\UpdateVersion.ps1 a
#   .\UpdateVersion.ps1 b
# ============================================

param(
    [Parameter(Mandatory = $true)]
    [ValidateSet('m', 'n', 'p', 'f', 'a', 'b')]
    [string]$Type
)

$ErrorActionPreference = "Stop"

# ============================================
# ŚCIEŻKI
# ============================================

$rootPath = $PSScriptRoot

$changelogPath = Join-Path $rootPath "CHANGELOG.md"
$csprojPath = Join-Path $rootPath "TTD.Main\TTD.Main.csproj"

# ============================================
# SPRAWDZENIE .CSPROJ
# ============================================

if (-not (Test-Path $csprojPath)) {

    Write-Host ""
    Write-Host "BLAD: Nie znaleziono pliku .csproj:"
    Write-Host $csprojPath
    Write-Host ""

    exit 1
}

# ============================================
# ODCZYT .CSPROJ
# ============================================

$csprojContent = Get-Content -Path $csprojPath -Raw

if ([string]::IsNullOrWhiteSpace($csprojContent)) {

    Write-Host ""
    Write-Host "BLAD: Plik .csproj jest pusty."
    Write-Host ""

    exit 1
}

# ============================================
# ODCZYT AKTUALNEJ WERSJI
# ============================================

$major = 0
$minor = 0
$patch = 0

$versionMatch = [regex]::Match(
    $csprojContent,
    '<Version>\s*(\d+)\.(\d+)\.(\d+)\s*</Version>'
)

if ($versionMatch.Success) {

    $major = [int]$versionMatch.Groups[1].Value
    $minor = [int]$versionMatch.Groups[2].Value
    $patch = [int]$versionMatch.Groups[3].Value

}
else {

    Write-Host ""
    Write-Host "Brak <Version> w .csproj."
    Write-Host "Ustawiam wersje poczatkowa: 0.0.0"
    Write-Host ""
}

# ============================================
# TYP ZMIANY
# ============================================

switch ($Type) {

    'm' {
        $major++
        $minor = 0
        $patch = 0

        $typeName = "MAJOR"
        $suffix = ""
    }

    'n' {
        $minor++
        $patch = 0

        $typeName = "MINOR"
        $suffix = ""
    }

    'p' {
        $patch++

        $typeName = "PATCH"
        $suffix = ""
    }

    'f' {
        $patch++

        $typeName = "FIX"
        $suffix = ""
    }

    'a' {
        $patch++

        $typeName = "ALPHA"
        $suffix = "-alpha"
    }

    'b' {
        $patch++

        $typeName = "BETA"
        $suffix = "-beta"
    }
}

# ============================================
# DATA I CZAS
# ============================================

$date = Get-Date -Format "yyyy.MM.dd"
$time = Get-Date -Format "HHmm"
$today = Get-Date -Format "yyyy-MM-dd"

# ============================================
# WERSJA SEMVER
# ============================================

$semVer = "$major.$minor.$patch"

# ============================================
# PEŁNA WERSJA
# ============================================

$version = "$date-$time-$semVer$suffix"

# ============================================
# TAG GIT
# ============================================

$tag = "v$version"

# ============================================
# AKTUALIZACJA VERSION
# ============================================

$versionPattern = '<Version>\s*.*?\s*</Version>'

if ([regex]::IsMatch($csprojContent, $versionPattern)) {

    $csprojContent = [regex]::Replace(
        $csprojContent,
        $versionPattern,
        "<Version>$semVer</Version>",
        1
    )

}
else {

    $propertyGroup = [regex]::Match(
        $csprojContent,
        '<PropertyGroup[^>]*>'
    )

    if (-not $propertyGroup.Success) {

        Write-Host ""
        Write-Host "BLAD: Nie znaleziono <PropertyGroup>."
        Write-Host ""

        exit 1
    }

    $position = $propertyGroup.Index + $propertyGroup.Length

    $insert = "`r`n    <Version>$semVer</Version>"

    $csprojContent = $csprojContent.Insert(
        $position,
        $insert
    )
}

# ============================================
# AKTUALIZACJA FILEVERSION
# ============================================

$fileVersionPattern = '<FileVersion>\s*.*?\s*</FileVersion>'

if ([regex]::IsMatch($csprojContent, $fileVersionPattern)) {

    $csprojContent = [regex]::Replace(
        $csprojContent,
        $fileVersionPattern,
        "<FileVersion>$semVer</FileVersion>",
        1
    )

}
else {

    $propertyGroup = [regex]::Match(
        $csprojContent,
        '<PropertyGroup[^>]*>'
    )

    $position = $propertyGroup.Index + $propertyGroup.Length

    $insert = "`r`n    <FileVersion>$semVer</FileVersion>"

    $csprojContent = $csprojContent.Insert(
        $position,
        $insert
    )
}

# ============================================
# AKTUALIZACJA ASSEMBLYVERSION
# ============================================

$assemblyVersionPattern = '<AssemblyVersion>\s*.*?\s*</AssemblyVersion>'

if ([regex]::IsMatch($csprojContent, $assemblyVersionPattern)) {

    $csprojContent = [regex]::Replace(
        $csprojContent,
        $assemblyVersionPattern,
        "<AssemblyVersion>$semVer</AssemblyVersion>",
        1
    )

}
else {

    $propertyGroup = [regex]::Match(
        $csprojContent,
        '<PropertyGroup[^>]*>'
    )

    $position = $propertyGroup.Index + $propertyGroup.Length

    $insert = "`r`n    <AssemblyVersion>$semVer</AssemblyVersion>"

    $csprojContent = $csprojContent.Insert(
        $position,
        $insert
    )
}

# ============================================
# ZAPIS .CSPROJ
# ============================================

Set-Content `
    -Path $csprojPath `
    -Value $csprojContent `
    -Encoding UTF8

Write-Host ""
Write-Host "OK: .csproj zaktualizowany"
Write-Host "SemVer: $semVer"

# ============================================
# CHANGELOG
# ============================================

$entry = @"
## [$version] - $today

### TYP ZMIANY: $typeName

### ZMIANY
- 

### POPRAWIONE
- 

---

"@

# ============================================
# UTWORZENIE CHANGELOG
# ============================================

if (-not (Test-Path $changelogPath)) {

    $newContent = @"
# CHANGELOG

$entry
"@

}
else {

    $content = Get-Content -Path $changelogPath -Raw

    if ([string]::IsNullOrWhiteSpace($content)) {

        $newContent = @"
# CHANGELOG

$entry
"@

    }
    else {

        $headerEnd = $content.IndexOf("## [")

        if ($headerEnd -lt 0) {

            $newContent =
                $content.TrimEnd() +
                "`r`n`r`n" +
                $entry

        }
        else {

            $newContent =
                $content.Substring(0, $headerEnd) +
                $entry +
                $content.Substring($headerEnd)
        }
    }
}

# ============================================
# ZAPIS CHANGELOG
# ============================================

Set-Content `
    -Path $changelogPath `
    -Value $newContent `
    -Encoding UTF8

Write-Host "OK: CHANGELOG zaktualizowany"

# ============================================
# PODSUMOWANIE
# ============================================

Write-Host ""
Write-Host "============================================"
Write-Host "              NOWA WERSJA"
Write-Host "============================================"
Write-Host ""

Write-Host "Typ zmiany: $typeName"
Write-Host "SemVer:     $semVer"
Write-Host "Wersja:     $version"
Write-Host "Git tag:    $tag"

# ============================================
# GIT
# ============================================

Write-Host ""
Write-Host "============================================"
Write-Host "              KOMENDY GIT"
Write-Host "============================================"
Write-Host ""

Write-Host "git add ."
Write-Host "git commit -m `"$tag`""
Write-Host "git tag $tag"
Write-Host "git push"
Write-Host "git push --tags"

Write-Host ""
Write-Host "Gotowe."