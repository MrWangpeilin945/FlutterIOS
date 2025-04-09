param (
    [switch]$DryRun,
    [Parameter(Mandatory = $true)][string]$SqlFileName,
    [string[]]$TenantKeys
)

$connectionInfo = @{
    Host = 'localhost'
    User = 'db_admin'
    Port = 15432
}

$securePassword = Read-Host -Prompt "[$($connectionInfo.User)] Password" -AsSecureString;
$standardPassword = [System.Runtime.InteropServices.Marshal]::PtrToStringAuto([System.Runtime.InteropServices.Marshal]::SecureStringToBSTR($securePassword))
$env:PGPASSWORD = $standardPassword

$rootDir = Split-Path -Parent (Split-Path -Parent $MyInvocation.MyCommand.Definition)

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

# メイン実行部分
$TenantKeys | ForEach-Object {
    $date = Get-Date
    $database = $_
    
    Write-Output "$($date.ToString('yyyy-MM-dd HH:mm:ss'))  [Tenant] $database [File] $SqlFileName"
    Invoke-PSQLFile -filePath $SqlFileName -database $database
}
