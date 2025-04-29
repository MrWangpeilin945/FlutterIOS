import 'package:flutter/foundation.dart';
import 'package:flutter/services.dart';

//Android/iOS プラットフォームを自動的に識別する
class SerialPlatform {
  static const MethodChannel _channel =
      MethodChannel('wellship.serial.channel');

  /// 检查当前设备蓝牙是否开启
  static Future<bool> isBluetoothEnabled() async {
    _PlatformUtils.assertMobile(); // 确保在移动平台上执行

    try {
      // 调用原生方法检查蓝牙是否开启
      final bool result = await _channel.invokeMethod('isBluetoothEnabled');
      debugPrint('蓝牙状态检查结果: $result'); // 输出到控制台
      return result;
    } on PlatformException catch (e) {
      // 处理平台异常，并将错误信息抛出
      debugPrint('蓝牙状态检查失败: ${e.message}'); // 输出错误信息
      throw Exception("${_PlatformUtils.platformPrefix}检查蓝牙状态失败: ${e.message}");
    }
  }
}

// プラットフォームの判定とモバイルプラットフォームのアサーション機能を提供する抽象クラス
abstract class _PlatformUtils {
  static bool get isAndroid => defaultTargetPlatform == TargetPlatform.android;
  static bool get isIOS => defaultTargetPlatform == TargetPlatform.iOS;
  static String get platformPrefix =>
      defaultTargetPlatform == TargetPlatform.android ? '[Android]' : '[iOS]';
  static void assertMobile() {
    if (!isAndroid && !isIOS) throw Exception('仅支持移动平台');
  }
}
