import Flutter
import UIKit

@main
@objc class AppDelegate: FlutterAppDelegate {
  override func application(
    _ application: UIApplication,
    didFinishLaunchingWithOptions launchOptions: [UIApplication.LaunchOptionsKey: Any]?
  ) -> Bool {
    GeneratedPluginRegistrant.register(with: self)
	
    // let controller: FlutterViewController = window?.rootViewController as! FlutterViewController

    // SerialApiSetup.setUp(binaryMessenger: controller.binaryMessenger, api: SerialApiImpl())

    let controller = window?.rootViewController as! FlutterViewController
    let channel = FlutterMethodChannel(name: "wellship.serial.channel", binaryMessenger: controller.binaryMessenger)

    channel.setMethodCallHandler { (call: FlutterMethodCall, result: @escaping FlutterResult) in
      switch call.method {
      case "open":
        print("🔌 iOS 原生接收到 open 方法调用")
        result("iOS Open Done")  // 可返回给 Flutter
      default:
        result(FlutterMethodNotImplemented)
      }
    }
	
    return super.application(application, didFinishLaunchingWithOptions: launchOptions)
  }
}

// // 实现 SerialApi 协议
// class SerialApiImpl: NSObject, SerialApi {
//     func open() -> String {
//         return "iOS側のopen()が呼ばれました！"
//     }
// }
