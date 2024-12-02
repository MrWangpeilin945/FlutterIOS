import 'dart:convert';

import 'package:auto_route/auto_route.dart';
import 'package:flutter/material.dart';
import 'package:hooks_riverpod/hooks_riverpod.dart';
import 'package:flutter_hooks/flutter_hooks.dart';
import 'package:typed_data/typed_buffers.dart';
import 'package:url_launcher/url_launcher.dart';
import 'package:usb_serial/usb_serial.dart';
import 'package:wellship_serial_client/data/model/behavior_settings.dart';
import 'package:wellship_serial_client/data/provider/wired_devices_provider.dart';
import 'package:wellship_serial_client/data/model/wired_settings.dart';

@RoutePage()
class WiredSerialCommunicationPage extends HookConsumerWidget {
  const WiredSerialCommunicationPage({super.key});

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final deviceStream = ref.watch(wiredDevicesProvider);
    final firstDevice = deviceStream.value?.firstOrNull;
    final serialSettings = ref.read(wiredSettingsProvider);
    final behaviorSettings = ref.read(behaviorSettingsProvider);

    final error = useState<String>("");
    final usbPort = useState<UsbPort?>(null);
    final text = useState<String>("");

    useEffect(() {
      if (firstDevice == null) {
        usbPort.value = null;
        return;
      }
      firstDevice.create().then((port) async {
        text.value = "";
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
        usbPort.value = port;
        final buffer = Uint8Buffer();
        // 多重に終了処理が行われないように
        bool aborting = false;
        port.inputStream!.listen((data) async {
          buffer.addAll(data);
          text.value = utf8.decode(buffer);
          // TODO ACK判定（ackTriggers, ackString）
          // TODO 停止判定（dataLength）
          // TODO 停止判定（eotString）
          // 停止判定
          if (!aborting && shouldAbort(buffer, behaviorSettings)) {
            aborting = true;
            final b = base64UrlEncode(buffer);
            final callback = behaviorSettings.callback;
            if (callback != null) {
              final result = {"value": b};
              final uri = callback.replace(queryParameters: result..addAll(callback.queryParameters));
              await launchUrl(uri, mode: LaunchMode.externalApplication);
            }
          }
        });
      }).onError((x, s) async {
        error.value = x.toString();
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
          Text("UsbPort:${usbPort.value}"),
          Text("text:${text.value}"),
          Text("error:${error.value}"),
        ]),
      ),
    );
  }

  bool shouldAbort(Uint8Buffer buffer, BehaviorSettings behaviorSettings) {
    final dataLength = behaviorSettings.dataLength;
    if (dataLength != null && buffer.length >= dataLength) {
      return true;
    }
    final eotString = behaviorSettings.eotString;
    if (eotString != null && utf8.decode(buffer).contains(eotString)) {
      return true;
    }
    return false;
  }
}
