$ErrorActionPreference = "Stop"

Write-Host "=== 1. NuGet package restore ==="
if (-not (Test-Path "nuget.exe")) {
    Write-Host "Downloading nuget.exe..."
    [Net.ServicePointManager]::SecurityProtocol = [Net.SecurityProtocolType]::Tls12
    Invoke-WebRequest -Uri "https://dist.nuget.org/win-x86-commandline/latest/nuget.exe" -OutFile "nuget.exe"
}
.\nuget.exe restore FormsAnswerChecker.sln

Write-Host "`n=== 2. Build with MSBuild ==="
$msbuild = "C:\Windows\Microsoft.NET\Framework64\v4.0.30319\MSBuild.exe"
if (-not (Test-Path $msbuild)) {
    $msbuild = "C:\Windows\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe"
}
& $msbuild FormsAnswerChecker.sln /p:Configuration=Debug /p:Platform="Any CPU" /v:minimal

if ($LASTEXITCODE -ne 0) {
    Write-Host "Build failed." -ForegroundColor Red
    exit $LASTEXITCODE
}

Write-Host "`n=== 3. Run MSTest ==="
$vstest = Get-ChildItem -Path "$env:USERPROFILE\.nuget\packages" -Recurse -Filter "vstest.console.exe" | Select-Object -First 1 -ExpandProperty FullName
if (-not $vstest -or -not (Test-Path $vstest)) {
    Write-Host "vstest.console.exe not found in NuGet cache. Check the installed Microsoft.TestPlatform package." -ForegroundColor Red
    exit 1
}

$testDll = "FormsAnswerChecker.Tests\bin\Debug\FormsAnswerChecker.Tests.dll"
if (-not (Test-Path $testDll)) {
    Write-Host "Test DLL not found: $testDll" -ForegroundColor Red
    exit 1
}

$adapterPackage = Get-ChildItem -Path "$env:USERPROFILE\.nuget\packages\mstest.testadapter" -Directory | Sort-Object Name -Descending | Select-Object -First 1
if (-not $adapterPackage) {
    Write-Host "MSTest.TestAdapter package not found in NuGet cache." -ForegroundColor Red
    exit 1
}

$buildDir = Join-Path $adapterPackage.FullName "build"
if (-not (Test-Path $buildDir)) {
    Write-Host "MSTest adapter build folder not found: $buildDir" -ForegroundColor Red
    exit 1
}

$testAdapterPath = Get-ChildItem -Path $buildDir -Directory | Where-Object { $_.Name -match '^(net46|net462)$' } | Select-Object -First 1 -ExpandProperty FullName
if (-not $testAdapterPath -or -not (Test-Path $testAdapterPath)) {
    Write-Host "MSTest adapter target folder not found under: $buildDir" -ForegroundColor Red
    exit 1
}

& $vstest $testDll /TestAdapterPath:$testAdapterPath /logger:"Console;verbosity=detailed"

if ($LASTEXITCODE -eq 0) {
    Write-Host "`nAll unit tests passed." -ForegroundColor Green
}
else {
    Write-Host "`nUnit tests failed." -ForegroundColor Red
    exit $LASTEXITCODE
}
