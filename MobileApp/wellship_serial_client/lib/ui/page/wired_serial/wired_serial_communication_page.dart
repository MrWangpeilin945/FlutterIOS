import 'dart:convert';

import 'package:auto_route/auto_route.dart';
import 'package:flutter/material.dart';
import 'package:hooks_riverpod/hooks_riverpod.dart';
import 'package:flutter_hooks/flutter_hooks.dart';
import 'package:typed_data/typed_buffers.dart';
import 'package:url_launcher/url_launcher.dart';
import 'package:usb_serial/usb_serial.dart';
import 'package:wellship_serial_client/data/provider/wired_devices_provider.dart';
import 'package:wellship_serial_client/data/provider/wired_settings.dart';

@RoutePage()
class WiredSerialCommunicationPage extends HookConsumerWidget {
  const WiredSerialCommunicationPage({super.key});

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final deviceStream = ref.watch(wiredDevicesProvider);
    final firstDevice = deviceStream.value?.firstOrNull;
    final serialSettings = ref.read(wiredSettingsProvider);

    final usbPort = useState<UsbPort?>(null);
    final text = useState<String>("");

    useEffect(() {
      if (firstDevice == null) {
        return;
      }
      firstDevice.create().then((port) async {
        text.value = "";
        usbPort.value = port;
        if (port == null) {
          return;
        }
        await port.open();
        await port.setDTR(serialSettings.useDtr);
        await port.setRTS(serialSettings.useRts);
        await port.setPortParameters(
          serialSettings.baud,
          serialSettings.dataBits,
          serialSettings.stopBits.value,
          serialSettings.parity.value,
        );
        final buffer = Uint8Buffer();
        // 多重に終了処理が行われないように
        bool aborting = false;
        port.inputStream!.listen((data) async {
          buffer.addAll(data);
          text.value = utf8.decode(buffer);
          // TODO ACK判定（ackTriggerStrings, ackString）
          // TODO 停止判定（dataLength）
          // TODO 停止判定（eotString）
          if (buffer.length > 40) {
            if (!aborting) {
              aborting = true;
              final b = base64UrlEncode(buffer);
              // TODO queryParameter.callbackを確認
              await launchUrl(Uri.parse('http://10.167.2.216/query-receiver.html?value=$b'),
                  mode: LaunchMode.externalApplication);
            }
          }
        });
      });
      return null;
    }, [firstDevice]);
    return Scaffold(
      appBar: AppBar(
        title: const Text('有線接続通信画面'),
        backgroundColor: Theme.of(context).colorScheme.inversePrimary,
      ),
      body: Center(
        child: Column(children: [
          Text("firstDevice:${firstDevice.toString()}"),
          Text("UsbPort:$usbPort"),
          Text("text:${text.value}"),
        ]),
      ),
    );
  }
}
