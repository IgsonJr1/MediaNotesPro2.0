param([string]$JellyfinVersion="10.10.7")
$root=Split-Path -Parent $MyInvocation.MyCommand.Path;$out=Join-Path $root "artifacts\MediaNotesPro";dotnet publish (Join-Path $root "src\MediaNotesPro\MediaNotesPro.csproj") -c Release -o $out -p:JellyfinVersion=$JellyfinVersion;Copy-Item (Join-Path $root "meta.json") $out -Force
