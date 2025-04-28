package jp.wellship.wellship_serial_client

import android.os.Bundle
import io.flutter.embedding.android.FlutterActivity
import io.flutter.plugin.common.MethodChannel
import io.flutter.embedding.engine.FlutterEngine
import jp.wellship.wellship_serial_client.SerialApi

class MainActivity: FlutterActivity() {
    override fun configureFlutterEngine(flutterEngine: FlutterEngine) {
        super.configureFlutterEngine(flutterEngine)

        SerialApi.setUp(flutterEngine.dartExecutor.binaryMessenger, SerialApiImpl())
    }
}

// 实现 SerialApi 接口
class SerialApiImpl : SerialApi {
    override fun open(): String {
        return "Android側のopen()が呼ばれました！"
    }
}
