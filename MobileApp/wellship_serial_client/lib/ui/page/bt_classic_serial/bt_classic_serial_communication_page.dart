import 'dart:convert';

import 'package:auto_route/auto_route.dart';
import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import 'package:flutter_bluetooth_serial/flutter_bluetooth_serial.dart';
import 'package:hooks_riverpod/hooks_riverpod.dart';
import 'package:flutter_hooks/flutter_hooks.dart';
import 'package:retry/retry.dart';
import 'package:typed_data/typed_buffers.dart';
import 'package:url_launcher/url_launcher.dart';
import 'package:wellship_serial_client/data/model/behavior_settings.dart';
import 'package:wellship_serial_client/data/model/bt_classic_settings.dart';
import 'package:wellship_serial_client/data/provider/behavior_settings_provider.dart';
import 'package:wellship_serial_client/data/provider/bt_classic_settings_provider.dart';
import 'package:wellship_serial_client/ui/component/wsc_bluetooth_device_select_dialog.dart';

@RoutePage()
class BtClassicSerialCommunicationPage extends HookConsumerWidget {
  const BtClassicSerialCommunicationPage({super.key});

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final serialSettings = ref.watch(btClassicSettingsProvider);
    final behaviorSettings = ref.read(behaviorSettingsProvider);
    final device = serialSettings.address != null
        ? BluetoothDevice(name: serialSettings.deviceName, address: serialSettings.address!)
        : null;

    final connection = useState<BluetoothConnection?>(null);
    final text = useState<String>("");

    useEffect(() {
      // デバイスを変更した際にコネクションを切断する
      connection.value?.dispose();
      connection.value = null;
      if (device == null) {
        return () {};
      }
      retry(
        () async {
          // 接続先のデバイスを切り替えたあと接続を試行しないようにする
          if (serialSettings.address != ref.read(btClassicSettingsProvider).address) {
            return;
          }
          final conn = await BluetoothConnection.toAddress(serialSettings.address);
          text.value = "";
          connection.value = conn;
          final buffer = Uint8Buffer();
          // 多重に終了処理が行われないように処理終了中かどうかのフラグを持つ
          bool aborting = false;
          int ackCount = 0;
          final ackTriggers = behaviorSettings.ackTriggers;
          final ackString = behaviorSettings.ackString;
          conn.input!.listen((data) async {
            buffer.addAll(data);
            text.value = utf8
                .decode(buffer)
                .replaceAllMapped(
                    RegExp(r'[\x00-\x20]'), (x) => String.fromCharCode(0x2400 + x.group(0)!.codeUnitAt(0)))
                .replaceAll(RegExp(r'[\x7f]'), String.fromCharCode(0x2421));
            if (ackTriggers != null && ackTriggers.isEmpty == false && ackString != null) {
              final checkText = utf8.decode(buffer);
              final count = RegExp(ackTriggers.first).allMatches(checkText).length;
              if (ackCount < count) {
                conn.output.add(utf8.encode(ackString));
                ackCount = count;
              }
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
        },
        maxAttempts: 99999,
        delayFactor: const Duration(seconds: 1),
        retryIf: (e) => e is PlatformException,
      );
      return () {
        connection.value?.dispose();
      };
    }, [device]);
    return Scaffold(
      appBar: AppBar(
        title: Text(behaviorSettings.title ?? 'BluetoothClassic接続通信画面'),
        backgroundColor: Theme.of(context).colorScheme.inversePrimary,
      ),
      body: Column(
        children: [
          Padding(
            padding: const EdgeInsets.all(16.0),
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Text(
                  '接続先',
                  style: Theme.of(context).textTheme.titleMedium,
                ),
                ListTile(
                    title: Text(serialSettings.deviceName ?? '未選択'),
                    subtitle: Text(serialSettings.address ?? ''),
                    trailing: const IconButton(
                      onPressed: null,
                      icon: Icon(Icons.settings),
                    ),
                    onTap: () async {
                      final device = await showDialog<BluetoothDevice>(
                        context: context,
                        builder: (context) {
                          return WscBluetoothDeviceSelectDialog(
                            title: '接続デバイスを選択してください',
                            groupValue: BluetoothDevice(
                              name: serialSettings.deviceName,
                              address: serialSettings.address ?? '',
                            ),
                          );
                        },
                      );
                      if (device != null) {
                        final settings = BtClassicSettings(
                          deviceName: device.name,
                          address: device.address,
                        );
                        ref.read(btClassicSettingsProvider.notifier).state = settings;
                        BtClassicSettings.saveSettings(settings);
                        // NOTE: 接続先が変更される場合、現在の接続を破棄します
                        connection.value?.dispose();
                        connection.value = null;
                      }
                    }),
                const SizedBox(height: 16),
                Row(
                  mainAxisAlignment: MainAxisAlignment.spaceBetween,
                  children: [
                    Container(
                      padding: const EdgeInsets.all(16),
                      decoration: BoxDecoration(
                        color: Colors.teal,
                        borderRadius: BorderRadius.circular(8),
                      ),
                      child: Text(
                        connection.value != null ? "準備完了！" : "準備中",
                        style: const TextStyle(
                          color: Colors.white,
                          fontWeight: FontWeight.bold,
                        ),
                      ),
                    ),
                  ],
                ),
              ],
            ),
          ),
          Expanded(
            child: Container(
              margin: const EdgeInsets.fromLTRB(16, 0, 16, 16),
              padding: const EdgeInsets.all(16),
              width: double.infinity,
              decoration: BoxDecoration(
                border: Border.all(color: Colors.teal.shade900),
                borderRadius: BorderRadius.circular(8),
              ),
              child: SelectableText(text.value),
            ),
          ),
        ],
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
