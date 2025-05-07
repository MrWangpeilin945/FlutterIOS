import 'package:flutter/foundation.dart';
import 'package:flutter/services.dart';

// Android/iOS プラットフォームを自動的に識別する
class SerialPlatform {
  static const MethodChannel _channel =
      MethodChannel('wellship.serial.channel');

  // Bluetoothの状態確認
  static Future<bool> isBluetoothEnabled() async {
    PlatformUtils.assertMobile();
    try {
      final bool result = await _channel.invokeMethod('isBluetoothEnabled');
      debugPrint('Bluetoothの状態: $result');
      return result;
    } on PlatformException catch (e) {
      debugPrint('Bluetoothの状態確認に失敗しました: ${e.message}');
      throw Exception(
          "${PlatformUtils.platformPrefix}Bluetoothの状態確認に失敗しました: ${e.message}");
    }
  }

  static Future<List<String>> scanForDevices() async {
    PlatformUtils.assertMobile();
    if (PlatformUtils.isAndroid) {
      return [];
    }
    try {
      final List<dynamic>? devices =
          await _channel.invokeMethod('scanForDevices');
      if (devices == null) {
        return [];
      }
      return devices
          .where((device) => device != null && device != "Unknown Device")
          .take(10)
          .map((device) => device.toString())
          .toList();
    } catch (e) {
      print("Bluetoothデバイスのスキャンに失敗しました: $e");
      return [];
    }
  }
}

// プラットフォームの判定とモバイルプラットフォームのアサーション機能を提供する抽象クラス
class PlatformUtils {
  static bool get isAndroid => defaultTargetPlatform == TargetPlatform.android;
  static bool get isIOS => defaultTargetPlatform == TargetPlatform.iOS;
  static String get platformPrefix =>
      defaultTargetPlatform == TargetPlatform.android ? '[Android]' : '[iOS]';
  static void assertMobile() {
    if (!isAndroid && !isIOS) throw Exception('モバイルプラットフォームのみ対応');
  }
}
