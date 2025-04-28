import 'package:flutter/foundation.dart';
import 'package:flutter/services.dart';

class SerialPlatform {
  static const MethodChannel _channel =
      MethodChannel('wellship.serial.channel');

  // 打开串口
  static Future<String?> open() async {
    if (defaultTargetPlatform == TargetPlatform.android) {
      // 如果是 Android 平台，调用 Android 端的实现
      try {
        final String result = await _channel.invokeMethod('open');
        return result;
      } on PlatformException catch (e) {
        return "Failed to invoke: '${e.message}'.";
      }
    } else if (defaultTargetPlatform == TargetPlatform.iOS) {
      // 如果是 iOS 平台，调用 iOS 端的实现
      try {
        final String result = await _channel.invokeMethod('open');
        return result;
      } on PlatformException catch (e) {
        return "Failed to invoke: '${e.message}'.";
      }
    } else {
      return 'Unsupported platform';
    }
  }
}
