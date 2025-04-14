import 'dart:async';
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
import 'package:wellship_serial_client/data/model/bt_classic_settings.dart';
import 'package:wellship_serial_client/data/provider/behavior_settings_provider.dart';
import 'package:wellship_serial_client/data/provider/bt_classic_settings_provider.dart';
import 'package:wellship_serial_client/ui/component/wsc_bluetooth_device_select_dialog.dart';
import 'package:wellship_serial_client/ui/component/wsc_control_char_escaped_selectable_text.dart';

@RoutePage()
class BtClassicSerialCommunicationPage extends HookConsumerWidget {
  const BtClassicSerialCommunicationPage({super.key});

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final serialSettings = ref.watch(btClassicSettingsProvider);
    final behaviorSettings = ref.watch(behaviorSettingsProvider);
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
          final conn = await BluetoothConnection.toAddress(serialSettings.address).timeout(const Duration(seconds: 3));
          text.value = "";
          connection.value = conn;
          final buffer = Uint8Buffer();
          // NOTE: 多重に終了処理が行われないように処理終了中かどうかのフラグを持つ
          bool aborting = false;
          conn.input!.listen(
            (data) async {
              buffer.addAll(data);
              text.value = utf8.decode(buffer);
              // Ack判定
              if (behaviorSettings.shouldAck(buffer, data)) {
                conn.output.add(utf8.encode(behaviorSettings.ackString ?? ''));
              }
              // 停止判定
              if (!aborting && behaviorSettings.shouldAbort(buffer)) {
                aborting = true;
                connection.value?.dispose();
                // flutterのbase64UrlEncodeは末尾の=を除去しないため手動で除去する
                final b = base64UrlEncode(buffer).replaceAll('=', '');
                final callback = behaviorSettings.callback;
                if (callback != null) {
                  final result = {"value": b};
                  final uri = callback.replace(queryParameters: result..addAll(callback.queryParameters));
                  await launchUrl(uri, mode: LaunchMode.externalApplication);
                }
              }
            },
          );
        },
        maxAttempts: 99999,
        // リクエスト間の最大待ち時間を1秒にしています
        maxDelay: const Duration(seconds: 1),
        retryIf: (e) => e is PlatformException || e is TimeoutException,
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
                  trailing: OutlinedButton(
                    child: const Text('接続先デバイス変更'),
                    onPressed: () async {
                      ref.read(btClassicSettingsProvider.notifier).state = const BtClassicSettings();
                      final device = await showDialog<BluetoothDevice>(
                        context: context,
                        builder: (context) {
                          return const WscBluetoothDeviceSelectDialog(
                            title: '接続デバイスを選択してください',
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
                    },
                  ),
                  onTap: null,
                ),
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
                        connection.value != null && (connection.value?.isConnected ?? false) ? "準備完了！" : "準備中",
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
              child: WscControlCharEscapedSelectableText(text.value),
            ),
          ),
        ],
      ),
    );
  }
}
