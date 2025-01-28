# 共通関数をインポート
$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Definition
$modulePath = Join-Path -Path $scriptDir -ChildPath "AWSCLIFunctions.psm1"
Import-Module -Name $modulePath

# 外部ファイルから設定を読み込む
$configPath = Join-Path -Path $scriptDir -ChildPath "config.json"
$config = Get-Content -Raw -Path $configPath | ConvertFrom-Json

$ec2InstanceId = $config.ec2InstanceId
$dbInstanceIdentifier = $config.dbInstanceIdentifier
$frontendEcsClusterName = $config.frontendEcsClusterName
$frontendEcsServiceName = $config.frontendEcsServiceName
$backendEcsClusterName = $config.backendEcsClusterName
$backendEcsServiceName = $config.backendEcsServiceName

# ローディングメッセージを表示
Write-Output "現在のステータスを取得しています。お待ちください..."

# 現在のステータスを取得
$ec2Status = Get-EC2Status -ec2InstanceId $ec2InstanceId
$rdsStatus = Get-RDSStatus -dbInstanceIdentifier $dbInstanceIdentifier
$frontendEcsTaskCount = Get-ECSTaskCount -clusterName $frontendEcsClusterName -serviceName $frontendEcsServiceName
$backendEcsTaskCount = Get-ECSTaskCount -clusterName $backendEcsClusterName -serviceName $backendEcsServiceName

# 現在のステータスを表示
Write-Output "[現在のステータス]"
Show-StatusTable `
    -ec2InstanceId $ec2InstanceId `
    -dbInstanceIdentifier $dbInstanceIdentifier `
    -frontendEcsServiceName $frontendEcsServiceName `
    -backendEcsServiceName $backendEcsServiceName `
    -ec2Status $ec2Status `
    -rdsStatus $rdsStatus `
    -frontendEcsTaskCount $frontendEcsTaskCount `
    -backendEcsTaskCount $backendEcsTaskCount

# サービスを起動してもよいか確認
$confirmation = Read-Host "サービスを起動しますか？ (y/n)"
if ($confirmation -ne "y") {
    Write-Output "サービスは起動されませんでした。"
    return
}

# 起動メッセージを表示
Write-Output "サービスを起動しています。お待ちください..."

# EC2インスタンスが停止中の場合は起動
if ($ec2Status -eq "stopped") {
    Write-Output "EC2インスタンスを起動しています: $($ec2InstanceId)"
    aws ec2 start-instances --instance-ids $($ec2InstanceId) --output json | Out-Null
}
else {
    Write-Output "EC2インスタンスは既に起動しています: $($ec2InstanceId)"
}

# RDSインスタンスが停止中の場合は起動
if ($rdsStatus -eq "stopped") {
    Write-Output "RDSインスタンスを起動しています: $($dbInstanceIdentifier)"
    aws rds start-db-instance --db-instance-identifier $($dbInstanceIdentifier) --output json | Out-Null
}
else {
    Write-Output "RDSインスタンスは既に起動しています: $($dbInstanceIdentifier)"
}

# フロントエンドECSサービスのタスク数を1に設定
if ($frontendEcsTaskCount -ne 1) {
    Write-Output "フロントエンドECSサービスを更新しています: $($frontendEcsServiceName) in cluster: $($frontendEcsClusterName) to desired count 1"
    aws ecs update-service --cluster $($frontendEcsClusterName) --service $($frontendEcsServiceName) --desired-count 1 --output json | Out-Null
}
else {
    Write-Output "フロントエンドECSサービスは既に設定されています: $($frontendEcsServiceName) in cluster: $($frontendEcsClusterName)"
}

# バックエンドECSサービスのタスク数を1に設定
if ($backendEcsTaskCount -ne 1) {
    Write-Output "バックエンドECSサービスを更新しています: $($backendEcsServiceName) in cluster: $($backendEcsClusterName) to desired count 1"
    aws ecs update-service --cluster $($backendEcsClusterName) --service $($backendEcsServiceName) --desired-count 1 --output json | Out-Null
}
else {
    Write-Output "バックエンドECSサービスは既に設定されています: $($backendEcsServiceName) in cluster: $($backendEcsClusterName)"
}

Write-Output "変更後のステータスを取得しています。お待ちください..."

# 変更後のステータスを取得
$ec2Status = Get-EC2Status -ec2InstanceId $ec2InstanceId
$rdsStatus = Get-RDSStatus -dbInstanceIdentifier $dbInstanceIdentifier
$frontendEcsTaskCount = Get-ECSTaskCount -clusterName $frontendEcsClusterName -serviceName $frontendEcsServiceName
$backendEcsTaskCount = Get-ECSTaskCount -clusterName $backendEcsClusterName -serviceName $backendEcsServiceName

# 変更後のステータスを表示
Write-Output "[変更後のステータス]"
Show-StatusTable `
    -ec2InstanceId $ec2InstanceId `
    -dbInstanceIdentifier $dbInstanceIdentifier `
    -frontendEcsServiceName $frontendEcsServiceName `
    -backendEcsServiceName $backendEcsServiceName `
    -ec2Status $ec2Status `
    -rdsStatus $rdsStatus `
    -frontendEcsTaskCount $frontendEcsTaskCount `
    -backendEcsTaskCount $backendEcsTaskCount
