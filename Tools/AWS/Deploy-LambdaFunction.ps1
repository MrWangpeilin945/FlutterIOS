# Lambda関数管理フォルダまでの相対パス
$lambdaFolderPath = "../../AWS/Lambda/"

# 更新対象のLambda関数名を入力
do {
    $targetLambdaFunction = Read-Host "デプロイ対象のLambda関数名を入力してください"
    # 入力を必須とする
    if (-not $targetLambdaFunction) {
        Write-Host "入力は必須です" -ForegroundColor Red
        continue
    }

    # 対象のLambda関数が存在するか確認
    $lambdaFunctionPath = Join-Path $lambdaFolderPath $targetLambdaFunction
    if (-not (Test-Path $lambdaFunctionPath)) {
        Write-Host "指定されたLambda関数は存在しません" -ForegroundColor Red
        $targetLambdaFunction = $null
        continue
    }
} while (-not $targetLambdaFunction)

$zipFilePath = "./$targetLambdaFunction.zip"

# フォルダをZIP圧縮
Compress-Archive -Path $lambdaFunctionPath -DestinationPath $zipFilePath -Force

# Lambda関数を更新
aws lambda update-function-code --function-name $targetLambdaFunction --zip-file fileb://$zipFilePath --output json | Out-Null

# zipファイルを削除
Remove-Item $zipFilePath

Write-Host "$targetLambdaFunction の更新が完了しました"