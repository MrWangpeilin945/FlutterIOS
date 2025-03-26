import json
import urllib.request

# S3のイベントをトリガーにしてECSタスク上のWebAPIを呼び出します。
def lambda_handler(event, context):
    # イベントから情報を取得
    s3_event = event['Records'][0]['s3']
    bucket_name = s3_event['bucket']['name']
    object_key = s3_event['object']['key']

    # TODO: バケット名からテナントキーを抽出する
    tenant_key = "stg01"
    base_url = f"https://{tenant_key}.wellship-stg.jp"
    endpoint = f"{base_url}/api/v1/external/dataImport/constantlyData"

    data = {
        'bucketName': bucket_name,
        'objectKey': object_key,
    }

    data = json.dumps(data).encode('utf-8')
    headers = {'Content-Type': 'application/json'}
    request = urllib.request.Request(endpoint, data, headers, method='POST')

    try:
        with urllib.request.urlopen(request) as response:
            response_body = response.read().decode('utf-8')
    except Exception as e:
        print(f"Error calling ALB: {e}")
    
    return {
        'statusCode': 200,
        'body': json.dumps('API call successful!')
    }
