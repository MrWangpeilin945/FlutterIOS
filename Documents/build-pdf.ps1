# PDFとして出力する

$docnames = @('アーキテクチャ設計書', 'API設計書', 'UI設計書')
$docnames | ForEach-Object {
    $dirName = $_
    $content = Get-Content "./$($dirName)/src/index.adoc"
    $version = (($content | Where-Object { $_ -like "*:revnumber: *" }) -replace ':revnumber: ').Trim()
    asciidoctor-pdf -a pdf-theme=./config/custom-theme.yml -r asciidoctor-diagram -r ./config/config.rb ./$dirName/src/index.adoc -o "$($dirName)_v$($version).pdf"
}
