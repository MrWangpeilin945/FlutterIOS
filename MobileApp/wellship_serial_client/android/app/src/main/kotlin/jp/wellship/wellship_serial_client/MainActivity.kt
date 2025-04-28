package jp.wellship.wellship_serial_client

import android.os.Bundle
import io.flutter.embedding.android.FlutterActivity
import io.flutter.plugin.common.MethodChannel
import io.flutter.embedding.engine.FlutterEngine
import jp.wellship.wellship_serial_client.SerialApi

class MainActivity: FlutterActivity() {
    // override fun configureFlutterEngine(flutterEngine: FlutterEngine) {
    //     super.configureFlutterEngine(flutterEngine)

    //     SerialApi.setUp(flutterEngine.dartExecutor.binaryMessenger, SerialApiImpl())
    // }

    private val CHANNEL = "wellship.serial.channel"

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)

        // 获取已初始化的 FlutterEngine 实例
        val flutterEngine: FlutterEngine = flutterEngine ?: return

        // 设置 MethodChannel 处理逻辑
        MethodChannel(flutterEngine.dartExecutor.binaryMessenger, CHANNEL).setMethodCallHandler { call, result ->
            if (call.method == "open") {
                // 在这里处理 'open' 方法
                // 执行实际的操作（如打开串口或其他设备）
                // 返回成功结果
                result.success("open 成功 from Android")  // 这里返回一个成功的消息
            } else {
                result.notImplemented()  // 如果 Flutter 端调用了一个未实现的方法
            }
        }
    }
}

// // 实现 SerialApi 接口
// class SerialApiImpl : SerialApi {
//     override fun open(): String {
//         return "Android側のopen()が呼ばれました！"
//     }
// }
