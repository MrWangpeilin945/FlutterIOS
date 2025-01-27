param (
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

$rootDir = Split-Path -Parent (Split-Path -Parent $MyInvocation.MyCommand.Definition)

$rawQuery = Get-Content "$($rootDir)/SQL/show-permissions.template.sql" -Raw
$embededQuery = ConvertFrom-MustacheTemplate -Template $rawQuery -Values @{TenantKey = $TenantKey }

# psqlで実行する
$psqlCommand = "psql -h $($connectionInfo.Host) -U $($connectionInfo.User) -d $TenantKey -p $($connectionInfo.Port) -c ""$embededQuery"""
Invoke-Expression $psqlCommand
