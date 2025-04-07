param (
    [string]$BaseName,
    [string[]]$TenantKeys
)

$rootDir = Split-Path -Parent (Split-Path -Parent $MyInvocation.MyCommand.Definition)

function Compare-Files {
    param (
        [string]$BaseName,
        [string]$TargetName
    )
    $baseFilePath = Join-Path $rootDir (Join-Path 'Schema' "$($BaseName).sql")
    $targetFilePath = Join-Path $rootDir (Join-Path 'Schema' "$($TargetName).sql")

    $baseContent = Get-Content $baseFilePath -Encoding utf8NoBOM
    $targetContent = Get-Content $targetFilePath -Encoding utf8NoBOM

    # 環境の差異を整形
    $targetContent = $targetContent -replace 'Owner: db_admin', 'Owner: {{owner}}'
    $targetContent = $targetContent -replace 'OWNER TO db_admin', 'OWNER TO  {{owner}}'
    $targetContent = $targetContent -replace '.*Dumped from database version.*'
    
    $baseContent = $baseContent -replace 'Owner: postgres', 'Owner: {{owner}}'
    $baseContent = $baseContent -replace 'OWNER TO postgres', 'OWNER TO  {{owner}}'
    $baseContent = $baseContent -replace '.*Dumped from database version.*'

    # 権限のSQLは無視するため切り取る
    $baseGrantRowIndex = ($baseContent | Select-String -Pattern 'GRANT ALL ON SCHEMA resultcollector TO*')[0].LineNumber
    $targetGrantRowIndex = ($targetContent | Select-String -Pattern 'GRANT ALL ON SCHEMA resultcollector TO*')[0].LineNumber
    $targetContent = $targetContent[0..($targetGrantRowIndex - 2)]
    $baseContent = $baseContent[0..($baseGrantRowIndex - 2)]

    # diffを計算する
    $diff = Compare-Object -ReferenceObject $baseContent -DifferenceObject $targetContent
    $diffCount = $diff.Length
    if ($diffCount -gt 0) {
        $diff | Out-File -FilePath "$($TargetName)_diff.txt"
    }
    $date = Get-Date
    Write-Output "$($date.ToString('yyyy-MM-dd HH:mm:ss')) [Tenant] $database : $diffCount diff."
}

# メイン実行部分
$TenantKeys | ForEach-Object {
    $database = $_
    Compare-Files -BaseName $BaseName -TargetName $database
}
