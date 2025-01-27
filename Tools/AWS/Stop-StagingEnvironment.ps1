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

# サービスを停止してもよいか確認
$confirmation = Read-Host "サービスを停止しますか？ (y/n)"
if ($confirmation -ne "y") {
    Write-Output "サービスは停止されませんでした。"
    return
}

# 停止メッセージを表示
Write-Output "サービスを停止しています。お待ちください..."

# EC2インスタンスが実行中の場合は停止
$ec2Job = $null
if ($ec2Status -eq "running") {
    Write-Output "EC2インスタンスを停止しています: $($ec2InstanceId)"
    aws ec2 stop-instances --instance-ids $($ec2InstanceId) --output json | Out-Null
    $ec2Job = Start-Job -ScriptBlock {
        param ($instanceId)
        aws ec2 wait instance-stopped --instance-ids $($instanceId) --output json | Out-Null
    } -ArgumentList $ec2InstanceId
}

# RDSインスタンスが利用可能な場合は停止
$rdsJob = $null
if ($rdsStatus -eq "available") {
    Write-Output "RDSインスタンスを停止しています: $($dbInstanceIdentifier)"
    aws rds stop-db-instance --db-instance-identifier $($dbInstanceIdentifier) --output json | Out-Null
    $rdsJob = Start-Job -ScriptBlock {
        param ($dbInstanceId)
        aws rds wait db-instance-stopped --db-instance-identifier $($dbInstanceId) --output json | Out-Null
    } -ArgumentList $dbInstanceIdentifier
}

# フロントエンドECSサービスのタスク数を0に設定
$frontendEcsJob = $null
if ($frontendEcsTaskCount -ne 0) {
    Write-Output "フロントエンドECSサービスを更新しています: $($frontendEcsServiceName) in cluster: $($frontendEcsClusterName) to desired count 0"
    aws ecs update-service --cluster $($frontendEcsClusterName) --service $($frontendEcsServiceName) --desired-count 0 --output json | Out-Null
    $frontendEcsJob = Start-Job -ScriptBlock {
        param ($clusterName, $serviceName)
        do {
            $taskCount = aws ecs describe-services --cluster $($clusterName) --services $($serviceName) --query 'services[0].runningCount' --output json | Out-Null
            Start-Sleep -Seconds 10
        } while ($taskCount -ne 0)
    } -ArgumentList $frontendEcsClusterName, $frontendEcsServiceName
}

# バックエンドECSサービスのタスク数を0に設定
$backendEcsJob = $null
if ($backendEcsTaskCount -ne 0) {
    Write-Output "バックエンドECSサービスを更新しています: $($backendEcsServiceName) in cluster: $($backendEcsClusterName) to desired count 0"
    aws ecs update-service --cluster $($backendEcsClusterName) --service $($backendEcsServiceName) --desired-count 0 --output json | Out-Null
    $backendEcsJob = Start-Job -ScriptBlock {
        param ($clusterName, $serviceName)
        do {
            $taskCount = aws ecs describe-services --cluster $($clusterName) --services $($serviceName) --query 'services[0].runningCount' --output json | Out-Null
            Start-Sleep -Seconds 10
        } while ($taskCount -ne 0)
    } -ArgumentList $backendEcsClusterName, $backendEcsServiceName
}

# すべてのジョブが完了するのを待つ
if ($ec2Job) { $ec2Job | Wait-Job }
if ($rdsJob) { $rdsJob | Wait-Job }
if ($frontendEcsJob) { $frontendEcsJob | Wait-Job }
if ($backendEcsJob) { $backendEcsJob | Wait-Job }

# 最終的なステータスを取得
$ec2Status = Get-EC2Status -ec2InstanceId $ec2InstanceId
$rdsStatus = Get-RDSStatus -dbInstanceIdentifier $dbInstanceIdentifier
$frontendEcsTaskCount = Get-ECSTaskCount -clusterName $frontendEcsClusterName -serviceName $frontendEcsServiceName
$backendEcsTaskCount = Get-ECSTaskCount -clusterName $backendEcsClusterName -serviceName $backendEcsServiceName

# 最終的なステータスを表示
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
