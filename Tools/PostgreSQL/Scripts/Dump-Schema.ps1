param (
    [switch]$DryRun,
    [Parameter(Mandatory = $true)][ValidateSet("AWS", "Local")][string]$Server,
    [string[]]$TenantKeys
)

$awsConnection = @{
    Host = 'localhost'
    User = 'db_admin'
    Port = 15432
}

$localConnection = @{
    Host = 'localhost'
    User = 'postgres'
    Port = 15433
}

$connectionInfo = switch ($Server) {
    'AWS' { $awsConnection }
    'Local' { $localConnection }
    default { @{} }
}


$securePassword = Read-Host -Prompt "[$($connectionInfo.User)] Password" -AsSecureString;
$standardPassword = [System.Runtime.InteropServices.Marshal]::PtrToStringAuto([System.Runtime.InteropServices.Marshal]::SecureStringToBSTR($securePassword))
$env:PGPASSWORD = $standardPassword

$rootDir = Split-Path -Parent (Split-Path -Parent $MyInvocation.MyCommand.Definition)

# pg_dumpコマンドを実行する関数
function Invoke-PgDump {
    param (
        [string]$database
    )

    $filePath = Join-Path $rootDir (Join-Path 'Schema' "$($database).sql")
    $command = "pg_dump -h $($connectionInfo.Host) -U $($connectionInfo.User) -p $($connectionInfo.Port) -d $database -s -F p -f ""$filePath"""
    
    if ($DryRun) {
        Write-Output "DryRun: $command"
    }
    else {
        New-Item -ItemType Directory -Path (Join-Path $rootDir 'Schema') -Force | Out-Null
        Invoke-Expression $command
    }
}

# メイン実行部分
$TenantKeys | ForEach-Object {
    $date = Get-Date
    $database = $_
    Invoke-PgDump -filePath $SqlFileName -database $database
    Write-Output "$($date.ToString('yyyy-MM-dd HH:mm:ss')) Dump completed. [Tenant] $database "
}
