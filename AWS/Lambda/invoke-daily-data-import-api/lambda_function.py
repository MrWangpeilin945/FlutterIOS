import json
import urllib.request

# EventBridge Schedulerによって定刻起動されECSタスク上のWebAPIを呼び出します。
def lambda_handler(event, context):
    # イベントから情報を取得
    bucket_name = event['bucketName']

    base_url = "https://stg01.wellship-stg.jp"
    endpoint = f"{base_url}/api/v1/external/dataImport/dailyData"

    data = {
        'bucketName': bucket_name,
        'objectKey': "",
    }

    data = json.dumps(data).encode('utf-8')
    headers = {'Content-Type': 'application/json'}
    request = urllib.request.Request(endpoint, data, headers, method='POST')
    
    try:
        with urllib.request.urlopen(request) as response:
            response_body = response.read().decode('utf-8')
    except urllib.error.URLError as e:
        print(f"Error calling ALB: {e}")
    
    return {
        'statusCode': 200,
        'body': json.dumps('API call successful!')
    }
