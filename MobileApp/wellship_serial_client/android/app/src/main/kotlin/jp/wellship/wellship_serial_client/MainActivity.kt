package jp.wellship.wellship_serial_client

import android.bluetooth.BluetoothAdapter
import android.bluetooth.BluetoothManager
import android.content.Context
import io.flutter.embedding.android.FlutterActivity
import io.flutter.embedding.engine.FlutterEngine
import io.flutter.plugin.common.MethodChannel

class MainActivity : FlutterActivity() {

    private val CHANNEL = "wellship.serial.channel"

    override fun configureFlutterEngine(flutterEngine: FlutterEngine) {
        super.configureFlutterEngine(flutterEngine)

        MethodChannel(flutterEngine.dartExecutor, CHANNEL).setMethodCallHandler { call, result ->
            when (call.method) {
                "isBluetoothEnabled" -> {
                    val isBluetoothEnabled = isBluetoothEnabled()
                    result.success(isBluetoothEnabled)
                }
                else -> result.notImplemented()
            }
        }
    }

    // Bluetoothの状態確認
    private fun isBluetoothEnabled(): Boolean {
        val bluetoothAdapter = getBluetoothAdapter()
        return bluetoothAdapter?.isEnabled ?: false
    }

    private fun getBluetoothAdapter(): BluetoothAdapter? {
        val bluetoothManager = getSystemService(Context.BLUETOOTH_SERVICE) as? BluetoothManager
        return bluetoothManager?.adapter
    }
}
