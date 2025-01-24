# 共通関数をインポート
Import-Module -Name .\AWSCLIFunctions.psm1

# 外部ファイルから設定を読み込む
$config = Get-Content -Raw -Path "config.json" | ConvertFrom-Json

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
$ec2Job = $null
if ($ec2Status -eq "stopped") {
    Write-Output "EC2インスタンスを起動しています: $($ec2InstanceId)"
    aws ec2 start-instances --instance-ids $($ec2InstanceId) --output json
    $ec2Job = Start-Job -ScriptBlock {
        param ($instanceId)
        aws ec2 wait instance-running --instance-ids $($instanceId) --output json
    } -ArgumentList $ec2InstanceId
}

# RDSインスタンスが停止中の場合は起動
$rdsJob = $null
if ($rdsStatus -eq "stopped") {
    Write-Output "RDSインスタンスを起動しています: $($dbInstanceIdentifier)"
    aws rds start-db-instance --db-instance-identifier $($dbInstanceIdentifier) --output json
    $rdsJob = Start-Job -ScriptBlock {
        param ($dbInstanceId)
        aws rds wait db-instance-available --db-instance-identifier $($dbInstanceId) --output json
    } -ArgumentList $dbInstanceIdentifier
}

# フロントエンドECSサービスのタスク数を1に設定
$frontendEcsJob = $null
if ($frontendEcsTaskCount -ne 1) {
    Write-Output "フロントエンドECSサービスを更新しています: $($frontendEcsServiceName) in cluster: $($frontendEcsClusterName) to desired count 1"
    aws ecs update-service --cluster $($frontendEcsClusterName) --service $($frontendEcsServiceName) --desired-count 1 --output json
    $frontendEcsJob = Start-Job -ScriptBlock {
        param ($clusterName, $serviceName)
        do {
            $taskCount = aws ecs describe-services --cluster $($clusterName) --services $($serviceName) --query 'services[0].runningCount' --output json
            Start-Sleep -Seconds 10
        } while ($taskCount -ne 1)
    } -ArgumentList $frontendEcsClusterName, $frontendEcsServiceName
}

# バックエンドECSサービスのタスク数を1に設定
$backendEcsJob = $null
if ($backendEcsTaskCount -ne 1) {
    Write-Output "バックエンドECSサービスを更新しています: $($backendEcsServiceName) in cluster: $($backendEcsClusterName) to desired count 1"
    aws ecs update-service --cluster $($backendEcsClusterName) --service $($backendEcsServiceName) --desired-count 1 --output json
    $backendEcsJob = Start-Job -ScriptBlock {
        param ($clusterName, $serviceName)
        do {
            $taskCount = aws ecs describe-services --cluster $($clusterName) --services $($serviceName) --query 'services[0].runningCount' --output json
            Start-Sleep -Seconds 10
        } while ($taskCount -ne 1)
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
Write-Output "[最終的なステータス]"
Show-StatusTable `
    -ec2InstanceId $ec2InstanceId `
    -dbInstanceIdentifier $dbInstanceIdentifier `
    -frontendEcsServiceName $frontendEcsServiceName `
    -backendEcsServiceName $backendEcsServiceName `
    -ec2Status $ec2Status `
    -rdsStatus $rdsStatus `
    -frontendEcsTaskCount $frontendEcsTaskCount `
    -backendEcsTaskCount $backendEcsTaskCount
