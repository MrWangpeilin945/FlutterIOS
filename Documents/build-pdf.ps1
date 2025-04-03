# PDFとして出力する

$docnames = @(
    # [PSCustomObject]@{Name = 'architecture'; JpName = 'アーキテクチャ設計書' },
    # [PSCustomObject]@{Name = 'ui-design'; JpName = 'UI設計書' },
    [PSCustomObject]@{Name = 'webapi-design'; JpName = 'API設計書' },
    [PSCustomObject]@{Name = 'mobileapp-design'; JpName = 'アプリ設計書' },
    [PSCustomObject]@{Name = 'equipment-design'; JpName = '機器連携設計書' },
    [PSCustomObject]@{Name = 'external-design'; JpName = '外部IF設計書' }
)

$docnames | ForEach-Object {
    $dirName = $_.JpName
    Set-Location $dirName
    $content = Get-Content "./src/index.adoc"
    $version = (($content | Where-Object { $_ -like "*:revnumber: *" }) -replace ':revnumber: ').Trim()
    $outputName = "$($_.Name)_v$($version).pdf"
    asciidoctor-pdf -a pdf-theme=../config/custom-theme.yml -r asciidoctor-diagram -r ../config/config.rb ./src/index.adoc -o $outputName
    scp $outputName aitel-ap:C:\inetpub\wwwroot\WELLSHIP\docs\pdf
    Set-Location ..
}
