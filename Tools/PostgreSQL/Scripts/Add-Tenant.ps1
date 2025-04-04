param (
    [switch]$DryRun,
    [Parameter(Mandatory = $true)][string]$TenantKey
)

Import-Module -Name PSMustache

$connectionInfo = @{
    Host = 'localhost'
    User = 'db_admin'
    Port = 15432
}

$securePassword = Read-Host -Prompt "[$($connectionInfo.User)] Password" -AsSecureString;
$standardPassword = [System.Runtime.InteropServices.Marshal]::PtrToStringAuto([System.Runtime.InteropServices.Marshal]::SecureStringToBSTR($securePassword))
$env:PGPASSWORD = $standardPassword

$tenantPassword = "$($TenantKey)_p@ssw0rd"

$rootDir = Split-Path -Parent (Split-Path -Parent $MyInvocation.MyCommand.Definition)

# psqlコマンドを実行する関数
function Invoke-PSQLCommand {
    param (
        [string]$query,
        [string]$database
    )
    $psqlCommand = "psql -h $($connectionInfo.Host) -U $($connectionInfo.User) -d $database -p $($connectionInfo.Port) -c ""$query"""
    
    if ($DryRun) {
        Write-Output "DryRun: $psqlCommand"
    }
    else {
        Invoke-Expression $psqlCommand
    }
}

# psqlコマンドでSQLファイルを実行する関数
function Invoke-PSQLFile {
    param (
        [string]$filePath,
        [string]$database
    )
    $fullPath = "$($rootDir)/SQL/$filePath"
    $psqlCommand = "psql -h $($connectionInfo.Host) -U $($connectionInfo.User) -d $database -p $($connectionInfo.Port) -f ""$fullPath"""
    
    if ($DryRun) {
        Write-Output "DryRun: $psqlCommand"
    }
    else {
        Invoke-Expression $psqlCommand
    }
}

# テンプレートからクエリを生成する関数
function ConvertFrom-Template {
    param (
        [string]$templatePath,
        [hashtable]$values
    )
    $rawQuery = Get-Content "$($rootDir)/SQL/$templatePath" -Raw
    return ConvertFrom-MustacheTemplate -Template $rawQuery -Values $values
}

# ロールを作成する
function New-TenantRole {
    $query = ConvertFrom-Template -templatePath "create-tenant-role.template.sql" -values @{TenantKey = $TenantKey; TenantPassword = $tenantPassword }
    Invoke-PSQLCommand -query $query -database "postgres"
}

# データベースを作成する
function New-TenantDatabase {
    $query = ConvertFrom-Template -templatePath "create-tenant-database.template.sql" -values @{TenantKey = $TenantKey }
    Invoke-PSQLCommand -query $query -database "postgres"
}

# スキーマを作成する
function New-Schema {
    $query = ConvertFrom-Template -templatePath "create-schema.template.sql" -values @{TenantKey = $TenantKey }
    Invoke-PSQLCommand -query $query -database $TenantKey
}

# テーブルを作成する
function New-Table {
    # Dockerコンテナの初期化用のDDLと共用するため、一時的にファイルに追記して実行します。
    # メモリ上でクエリを渡すと文字化けするので、一度ファイルに落としています。
    $createTable = Get-Content -Path "$rootDir/../../Database/Docker/init/02_create_table.sql"  -Encoding utf8NoBOM
    
    # Dockerコンテナ用の記述を消す
    $ddl = $createTable | Where-Object { $_ -notlike '\c*' } | Where-Object { $_ -notlike '-- DB切り替え*' }
    
    $header = Get-Content -Path "$rootDir/SQL/create-table.template.sql" -Encoding utf8NoBOM
    $header + $ddl | Out-File -FilePath "$rootDir/SQL/create-table.template.sql"
    
    # SQLを実行する
    Invoke-PSQLFile -filePath "create-table.template.sql" -database $TenantKey
    
    # SQLを元に戻す
    $header | Out-File -FilePath "$rootDir/SQL/create-table.template.sql" -Encoding utf8NoBOM
}

# 権限を付与する
function Grant-TenantPermissions {
    $query = ConvertFrom-Template -templatePath "grant-tenant.template.sql" -values @{TenantKey = $TenantKey }
    Invoke-PSQLCommand -query $query -database $TenantKey
}

# メイン実行部分
New-TenantRole
New-TenantDatabase
New-Schema
New-Table
Grant-TenantPermissions
