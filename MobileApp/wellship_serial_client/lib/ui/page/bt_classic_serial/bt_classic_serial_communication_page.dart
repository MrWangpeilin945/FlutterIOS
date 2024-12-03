import 'dart:convert';

import 'package:auto_route/auto_route.dart';
import 'package:flutter/material.dart';
import 'package:flutter_bluetooth_serial/flutter_bluetooth_serial.dart';
import 'package:hooks_riverpod/hooks_riverpod.dart';
import 'package:flutter_hooks/flutter_hooks.dart';
import 'package:typed_data/typed_buffers.dart';
import 'package:url_launcher/url_launcher.dart';
import 'package:wellship_serial_client/data/model/behavior_settings.dart';
import 'package:wellship_serial_client/data/provider/bt_classic_devices_provider.dart';
import 'package:wellship_serial_client/data/model/bt_classic_settings.dart';

@RoutePage()
class BtClassicSerialCommunicationPage extends HookConsumerWidget {
  const BtClassicSerialCommunicationPage({super.key});

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final deviceStream = ref.watch(btClassicDevicesProvider);
    // TODO 設定できるようにする。
    // TODO デバイス名よりもアドレスで判別したほうがよい
    const deviceName = 'D1-N273';
    final device = deviceStream.value?.where((x) => x.device.name == deviceName).firstOrNull?.device;
    final serialSettings = ref.read(btClassicSettingsProvider);
    final behaviorSettings = ref.read(behaviorSettingsProvider);

    final connection = useState<BluetoothConnection?>(null);
    final text = useState<String>("");

    useEffect(() {
      if (device == null) {
        return () {
          connection.value?.dispose();
        };
      }
      BluetoothConnection.toAddress(serialSettings.address).then((conn) async {
        text.value = "";
        connection.value = conn;
        final buffer = Uint8Buffer();
        // 多重に終了処理が行われないように
        bool aborting = false;
        conn.input!.listen((data) async {
          buffer.addAll(data);
          text.value = utf8.decode(buffer);
          // TODO ACK判定（ackTriggers, ackString）
          if (behaviorSettings.ackString != null && behaviorSettings.ackTriggers?.isNotEmpty == true) {
            final ackString = behaviorSettings.ackString ?? "";
          }
          // 停止判定
          if (!aborting && shouldAbort(buffer, behaviorSettings)) {
            aborting = true;
            connection.value?.dispose();
            final b = base64UrlEncode(buffer);
            final callback = behaviorSettings.callback;
            if (callback != null) {
              final result = {"value": b};
              final uri = callback.replace(queryParameters: result..addAll(callback.queryParameters));
              await launchUrl(uri, mode: LaunchMode.externalApplication);
            }
          }
        });
      });
      return () {
        connection.value?.dispose();
      };
    }, [serialSettings.address]);
    return Scaffold(
      appBar: AppBar(
        title: Text(behaviorSettings.title ?? 'BluetoothClassic接続通信画面'),
        backgroundColor: Theme.of(context).colorScheme.inversePrimary,
      ),
      body: Center(
        child: Column(children: [
          Text("targetDevice:${device?.name} (${device?.address})"),
          Text("connection:${connection.value}"),
          Text(
            connection.value != null ? "準備完了！" : "準備中",
            style: const TextStyle(fontSize: 28),
          ),
          Text("text:${text.value}"),
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
