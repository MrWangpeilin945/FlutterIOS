import 'package:permission_handler/permission_handler.dart';

enum BluetoothConnectPermissionStatus { granted, denied, permanentlyDenied, restricted }

class BluetoothPermissionsHandler {
  Future<bool> get isGranted async {
    final status = await Permission.bluetoothScan.status;
    switch (status) {
      case PermissionStatus.granted:
      case PermissionStatus.limited:
        return true;
      case PermissionStatus.denied:
      case PermissionStatus.permanentlyDenied:
      case PermissionStatus.restricted:
        return false;
      default:
        return false;
    }
  }

  Future<PermissionStatus> requestScan() async => await Permission.bluetoothScan.request();
  Future<PermissionStatus> requestConnect() async => await Permission.bluetoothConnect.request();
}
