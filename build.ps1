$ErrorActionPreference = "Stop"

Write-Host "=== 1. NuGet パッケージの復元 ==="
if (-not (Test-Path "nuget.exe")) {
    Write-Host "nuget.exe をダウンロードしています..."
    [Net.ServicePointManager]::SecurityProtocol = [Net.SecurityProtocolType]::Tls12
    Invoke-WebRequest -Uri "https://dist.nuget.org/win-x86-commandline/latest/nuget.exe" -OutFile "nuget.exe"
}

Write-Host "パッケージを復元中..."
.\nuget.exe restore FormsAnswerChecker.sln

Write-Host "=== 2. MSBuild によるビルド ==="
$msbuild = "C:\Windows\Microsoft.NET\Framework64\v4.0.30319\MSBuild.exe"
if (-not (Test-Path $msbuild)) {
    $msbuild = "C:\Windows\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe"
}

& $msbuild FormsAnswerChecker.sln /p:Configuration=Debug /p:Platform="Any CPU" /v:minimal

Write-Host "=== 3. ビルド完了確認 ==="
$exePath = "FormsAnswerChecker\bin\Debug\FormsAnswerChecker.exe"
if (Test-Path $exePath) {
    Write-Host "ビルド成功: $exePath" -ForegroundColor Green
} else {
    Write-Host "ビルド失敗: 実行ファイルが見つかりません" -ForegroundColor Red
}
