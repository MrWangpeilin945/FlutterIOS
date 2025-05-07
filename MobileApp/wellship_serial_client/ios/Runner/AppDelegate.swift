import Flutter
import UIKit
import CoreBluetooth

@main
@objc class AppDelegate: FlutterAppDelegate, CBCentralManagerDelegate {

    var centralManager: CBCentralManager?
    var flutterResult: FlutterResult?
    var peripherals: [CBPeripheral] = []
    private var callMethod: String = ""

    override func application(
        _ application: UIApplication,
        didFinishLaunchingWithOptions launchOptions: [UIApplication.LaunchOptionsKey: Any]?
    ) -> Bool {
        GeneratedPluginRegistrant.register(with: self)
        
        let controller = window?.rootViewController as! FlutterViewController
        let channel = FlutterMethodChannel(name: "wellship.serial.channel", binaryMessenger: controller.binaryMessenger)

        channel.setMethodCallHandler { [weak self] (call, result) in
            self?.callMethod = call.method
            guard let self = self else { return }

            switch call.method {
            case "isBluetoothEnabled":
                print("🔌 iOSネイティブ側で isBluetoothEnabled メソッドが呼び出されました")
                self.flutterResult = result
                self.centralManager = CBCentralManager(delegate: self, queue: nil)

            case "scanForDevices":
                print("🔍 iOSネイティブ側で scanForDevices メソッドが呼び出されました")
                self.flutterResult = result
                self.peripherals.removeAll()
                self.centralManager = CBCentralManager(delegate: self, queue: nil)

            default:
                result(FlutterMethodNotImplemented)
            }
        }

        return super.application(application, didFinishLaunchingWithOptions: launchOptions)
    }

    @objc func centralManagerDidUpdateState(_ central: CBCentralManager) {
        guard let result = flutterResult else { return }
        
        switch central.state {
        case .poweredOn:
            print("✅ Bluetoothが有効です")
            if callMethod == "isBluetoothEnabled" {
                result(true)
            } else if callMethod == "scanForDevices" {
                central.scanForPeripherals(withServices: nil, options: nil)
                DispatchQueue.main.asyncAfter(deadline: .now() + 10.0) {
                    central.stopScan()
                    let deviceList = self.peripherals
                        .compactMap { $0.name }
                        .filter { !$0.isEmpty && $0 != "Unknown Device" }
                        .prefix(10)
                    result(Array(deviceList))
                }
            }
            
        case .poweredOff:
            print("❌ Bluetoothが無効です")
            result(callMethod == "isBluetoothEnabled" ? false : [])
            
        default:
            print("⚠️ Bluetoothの状態が不明です")
            result(callMethod == "isBluetoothEnabled" ? false : [])
        }
        
        flutterResult = nil
    }

    func centralManager(_ central: CBCentralManager, 
                   didDiscover peripheral: CBPeripheral,
                   advertisementData: [String: Any], 
                   rssi RSSI: NSNumber) {

    let deviceName = peripheral.name ?? "Unnamed"
    print("デバイスを発見: \(deviceName), RSSI: \(RSSI), UUID: \(peripheral.identifier)")
    
    if !peripherals.contains(where: { $0.identifier == peripheral.identifier }) {
        peripherals.append(peripheral)
        
        let names = peripherals.compactMap { $0.name }
        print("デバイス一覧：\(names)")
    }
}
}