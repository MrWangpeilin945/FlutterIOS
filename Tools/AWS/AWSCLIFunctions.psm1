# EC2インスタンスのステータスを取得する関数
function Get-EC2Status {
    param ([string]$ec2InstanceId)
    $status = aws ec2 describe-instances --instance-ids $($ec2InstanceId) --query 'Reservations[0].Instances[0].State.Name' --output json
    return $status
}

# RDSインスタンスのステータスを取得する関数
function Get-RDSStatus {
    param ([string]$dbInstanceIdentifier)
    $status = aws rds describe-db-instances --db-instance-identifier $($dbInstanceIdentifier) --query 'DBInstances[0].DBInstanceStatus' --output json
    return $status
}

# ECSサービスのタスク数を取得する関数
function Get-ECSTaskCount {
    param (
        [string]$clusterName,
        [string]$serviceName
    )
    $taskCount = aws ecs describe-services --cluster $($clusterName) --services $($serviceName) --query 'services[0].runningCount' --output json
    return $taskCount
}

# ステータステーブルを表示する関数
function Show-StatusTable {
    param (
        [string]$ec2InstanceId,
        [string]$dbInstanceIdentifier,
        [string]$frontendEcsServiceName,
        [string]$backendEcsServiceName,
        [string]$ec2Status,
        [string]$rdsStatus,
        [string]$frontendEcsTaskCount,
        [string]$backendEcsTaskCount
    )
    $statusTable = @(
        [PSCustomObject]@{名称 = "EC2"; 識別子 = $ec2InstanceId; ステータス = $ec2Status }
        [PSCustomObject]@{名称 = "RDS"; 識別子 = $dbInstanceIdentifier; ステータス = $rdsStatus }
        [PSCustomObject]@{名称 = "Frontend ECS"; 識別子 = $frontendEcsServiceName; ステータス = "Tasks ($frontendEcsTaskCount)" }
        [PSCustomObject]@{名称 = "Backend ECS"; 識別子 = $backendEcsServiceName; ステータス = "Tasks ($backendEcsTaskCount)" }
    )
    $statusTable | Format-Table -AutoSize
}