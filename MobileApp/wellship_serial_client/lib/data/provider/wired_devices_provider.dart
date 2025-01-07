import 'package:hooks_riverpod/hooks_riverpod.dart';
import 'package:usb_serial/usb_serial.dart';

final wiredDevicesProvider = StreamProvider<List<UsbDevice>>((ref) async* {
  List<UsbDevice> devices = await UsbSerial.listDevices();
  yield devices;
  final stream = UsbSerial.usbEventStream;
  if (stream == null) {
    return;
  }
  await for (final msg in stream) {
    final device = msg.device;
    if (device != null) {
      if (msg.event == UsbEvent.ACTION_USB_ATTACHED) {
        devices.add(device);
      } else if (msg.event == UsbEvent.ACTION_USB_DETACHED) {
        devices.remove(device);
      }
      yield devices;
    }
  }
});
