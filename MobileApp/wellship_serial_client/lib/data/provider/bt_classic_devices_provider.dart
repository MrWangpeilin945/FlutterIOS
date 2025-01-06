import 'package:flutter_bluetooth_serial/flutter_bluetooth_serial.dart';
import 'package:hooks_riverpod/hooks_riverpod.dart';

final btClassicDevicesProvider = StreamProvider<List<BluetoothDiscoveryResult>>((ref) async* {
  List<BluetoothDiscoveryResult> devices = [];
  yield devices;
  await FlutterBluetoothSerial.instance.cancelDiscovery();
  await for (BluetoothDiscoveryResult result in FlutterBluetoothSerial.instance.startDiscovery()) {
    if (!devices.any((device) => device.device.address == result.device.address) && result.device.name != null) {
      devices.add(result);
    }
    yield devices;
  }
});
