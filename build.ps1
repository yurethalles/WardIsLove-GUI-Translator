param(
    [string]$ValheimPath = 'C:\Program Files (x86)\Steam\steamapps\common\Valheim',
    [string]$BepInExPath = 'C:\Users\Usuario\AppData\Roaming\com.kesomannen.gale\valheim\profiles\Glitnir Awakening 2\BepInEx',
    [string]$Compiler = 'C:\Windows\Microsoft.NET\Framework64\v4.0.30319\csc.exe'
)
$ErrorActionPreference = 'Stop'
$managed = Join-Path $ValheimPath 'valheim_Data\Managed'
$references = @(
    (Join-Path $managed 'mscorlib.dll'), (Join-Path $managed 'System.dll'),
    (Join-Path $managed 'System.Core.dll'), (Join-Path $managed 'netstandard.dll'),
    (Join-Path $managed 'UnityEngine.dll'), (Join-Path $managed 'UnityEngine.CoreModule.dll'), (Join-Path $managed 'Newtonsoft.Json.dll'),
    (Join-Path $BepInExPath 'core\BepInEx.dll'), (Join-Path $BepInExPath 'core\0Harmony.dll')
)
foreach ($file in @($Compiler) + $references) {
    if (-not (Test-Path -LiteralPath $file -PathType Leaf)) { throw "Arquivo necessário não encontrado: $file" }
}
$outDir = Join-Path $PSScriptRoot 'dist'
New-Item -ItemType Directory -Force -Path $outDir | Out-Null
$output = Join-Path $outDir 'WardIsLove.GUITranslator.dll'
$argsCompiler = @('/nologo', '/noconfig', '/nostdlib', '/target:library', '/codepage:65001', "/out:$output")
$argsCompiler += $references | ForEach-Object { "/reference:$_" }
$argsCompiler += Join-Path $PSScriptRoot 'WardTraducao.cs'
& $Compiler @argsCompiler
if ($LASTEXITCODE -ne 0) { throw "Falha ao compilar (código $LASTEXITCODE)." }
Write-Host "Complemento criado: $output"
