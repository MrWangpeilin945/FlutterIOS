# HTMLとして出力する

$docnames = @(
    [PSCustomObject]@{Name = 'architecture'; JpName = 'アーキテクチャ設計書' },
    [PSCustomObject]@{Name = 'webapi-design'; JpName = 'API設計書' },
    [PSCustomObject]@{Name = 'ui-design'; JpName = 'UI設計書' }
)

$docnames | ForEach-Object {
    $name = $_.Name
    Set-Location $_.JpName
    asciidoctor "./src/index.adoc" -a stylesheet=wellshipstyle.css --destination-dir "./dist/" -r asciidoctor-diagram
    Copy-Item -Recurse -Force ./src/images -Destination ./dist/.
    scp -r "./dist/images" "aitel-ap:C:\inetpub\wwwroot\WELLSHIP\docs\$($name)"
    scp "./dist/index.html" "aitel-ap:C:\inetpub\wwwroot\WELLSHIP\docs\$($name)\index.html"
    Set-Location ..
}
