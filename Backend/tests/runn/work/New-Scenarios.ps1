$templ = Get-Content template.yml -encoding utf8 -Raw
$data = Get-Content input.csv -encoding utf8 | ConvertFrom-Csv -Header @('APIID', 'ケースID', 'ケース名', 'メソッド', 'パス', '区分', 'リクエストヘッダ', 'ステータス')

$data | ForEach-Object {
    $d = $_
    $text = ConvertFrom-MustacheTemplate -Template $templ -Values $d
    $text = $text -replace '「', '{' -replace '」', '}'
    $filePath = "../$($d.APIID)/Scenarios/$($d.ケースID).yml"
    $text | Out-File -FilePath $filePath -Force
}