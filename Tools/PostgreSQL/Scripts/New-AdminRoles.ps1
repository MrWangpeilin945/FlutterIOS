param (
    [switch]$DryRun
)

Import-Module -Name PSMustache

$connectionInfo = @{
    Host = 'localhost'
    User = 'wellship_master'
    Port = 15432
}

$securePassword = Read-Host -Prompt "[$($connectionInfo.User)] Password" -AsSecureString;
$standardPassword = [System.Runtime.InteropServices.Marshal]::PtrToStringAuto([System.Runtime.InteropServices.Marshal]::SecureStringToBSTR($securePassword))
$env:PGPASSWORD = $standardPassword

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

$query = Get-Content "$($rootDir)/SQL/initial-setup.sql" -Raw

Invoke-PSQLCommand -query $query -database 'postgres'
