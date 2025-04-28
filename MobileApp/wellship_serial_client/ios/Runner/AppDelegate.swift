import Flutter
import UIKit
import CoreBluetooth

@main
@objc class AppDelegate: FlutterAppDelegate {

  var centralManager: CBCentralManager?
  var flutterResult: FlutterResult?  // 保存Flutter回调

  override func application(
    _ application: UIApplication,
    didFinishLaunchingWithOptions launchOptions: [UIApplication.LaunchOptionsKey: Any]?
  ) -> Bool {
    GeneratedPluginRegistrant.register(with: self)
	
    // let controller: FlutterViewController = window?.rootViewController as! FlutterViewController

    // SerialApiSetup.setUp(binaryMessenger: controller.binaryMessenger, api: SerialApiImpl())

    let controller = window?.rootViewController as! FlutterViewController
    let channel = FlutterMethodChannel(name: "wellship.serial.channel", binaryMessenger: controller.binaryMessenger)

    // channel.setMethodCallHandler { (call: FlutterMethodCall, result: @escaping FlutterResult) in
    //   switch call.method {
    //   case "open":
    //     print("🔌 iOS 原生接收到 open 方法调用")
    //     result("iOS Open Done")  // 可返回给 Flutter
    //   default:
    //     result(FlutterMethodNotImplemented)
    //   }
    // }

    channel.setMethodCallHandler { [weak self] (call: FlutterMethodCall, result: @escaping FlutterResult) in
      guard let self = self else { return }

      switch call.method {
      case "open":
        print("🔌 iOS 原生接收到 open 方法调用")
        
        // 初始化 CoreBluetooth
        self.flutterResult = result
        self.centralManager = CBCentralManager(delegate: self, queue: nil)

      default:
        result(FlutterMethodNotImplemented)
      }
	  }

    return super.application(application, didFinishLaunchingWithOptions: launchOptions)
  }

    // CBCentralManagerDelegate 回调
  func centralManagerDidUpdateState(_ central: CBCentralManager) {
    switch central.state {
    case .poweredOn:
      print("✅ Bluetooth 已开启，可以开始扫描设备")
      
      // 这里可以开始扫描设备（比如）
      central.scanForPeripherals(withServices: nil, options: nil)

      // 回传 Flutter
      flutterResult?("Bluetooth is powered on")
      flutterResult = nil  // 回传完清空
    case .poweredOff:
      print("❌ Bluetooth 未开启")
      flutterResult?("Bluetooth is powered off")
      flutterResult = nil
    default:
      print("⚠️ Bluetooth 状态未知: \(central.state.rawValue)")
      flutterResult?("Bluetooth state: \(central.state.rawValue)")
      flutterResult = nil
    }
}
// // 实现 SerialApi 协议
// class SerialApiImpl: NSObject, SerialApi {
//     func open() -> String {
//         return "iOS側のopen()が呼ばれました！"
//     }
// }
