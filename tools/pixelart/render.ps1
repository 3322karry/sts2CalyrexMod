# 用 LibreSprite 的 JS 脚本绘制 32x32 像素图标，再最近邻放大到 128x128。
# 用法：pwsh -File tools\pixelart\render.ps1 -Name black_white_carrot [-OutFile ...] [-NoUpscale]
[CmdletBinding()]
param(
  [Parameter(Mandatory = $true)][string]$Name,
  [string]$OutFile,
  [switch]$NoUpscale,
  [int]$Scale = 4
)

$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent (Split-Path -Parent $PSScriptRoot)   # 仓库根
$toolDir = $PSScriptRoot
$libre = 'D:\vibeprograms\mcp-servers\pixel-mcp\libresprite\app\libresprite.exe'
$work = 'D:\vibeprograms\mcp-servers\pixel-mcp\temp\calyrex'
New-Item -ItemType Directory -Force $work | Out-Null

if (-not (Test-Path $libre)) { throw "LibreSprite 不存在: $libre" }
if (-not $OutFile) { $OutFile = Join-Path $toolDir "preview\$Name.png" }
New-Item -ItemType Directory -Force (Split-Path -Parent $OutFile) | Out-Null

Add-Type -AssemblyName System.Drawing

# 空白 32x32 画布（透明）
$canvas = Join-Path $work 'canvas32.png'
if (-not (Test-Path $canvas)) {
  $bmp = New-Object System.Drawing.Bitmap(32, 32)
  $g = [System.Drawing.Graphics]::FromImage($bmp)
  $g.Clear([System.Drawing.Color]::Transparent)
  $g.Dispose()
  $bmp.Save($canvas, [System.Drawing.Imaging.ImageFormat]::Png)
  $bmp.Dispose()
}

$small = Join-Path $work "$Name-32.png"
$script = Join-Path $work "$Name.js"
$lib = Get-Content (Join-Path $toolDir 'lib.js') -Raw
$icons = Get-Content (Join-Path $toolDir 'icons.js') -Raw
$body = $lib + "`n" + $icons + "`npInit(32, 32);`nicon_$Name();`npFlush();`napp.activeSprite.saveAs('" + ($small -replace '\\', '/') + "');`n"
[IO.File]::WriteAllText($script, $body, (New-Object System.Text.UTF8Encoding($false)))

$env:APPDATA = 'D:\vibeprograms\mcp-servers\pixel-mcp\libresprite\appdata'
Remove-Item $small -Force -ErrorAction SilentlyContinue
$log = & $libre -b $canvas --script $script 2>&1 | Out-String
if (-not (Test-Path $small)) { throw "渲染失败（$Name）：`n$log" }

if ($NoUpscale -or $Scale -le 1) {
  Copy-Item $small $OutFile -Force
} else {
  $src = [System.Drawing.Bitmap]::FromFile($small)
  $dst = New-Object System.Drawing.Bitmap(($src.Width * $Scale), ($src.Height * $Scale))
  $g2 = [System.Drawing.Graphics]::FromImage($dst)
  $g2.InterpolationMode = [System.Drawing.Drawing2D.InterpolationMode]::NearestNeighbor
  $g2.PixelOffsetMode = [System.Drawing.Drawing2D.PixelOffsetMode]::Half
  $g2.DrawImage($src, 0, 0, $dst.Width, $dst.Height)
  $g2.Dispose()
  $dst.Save($OutFile, [System.Drawing.Imaging.ImageFormat]::Png)
  $dst.Dispose(); $src.Dispose()
}
Write-Output $OutFile
