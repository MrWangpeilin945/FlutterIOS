import 'dart:convert';

import 'package:auto_route/auto_route.dart';
import 'package:flutter/material.dart';
import 'package:hooks_riverpod/hooks_riverpod.dart';
import 'package:flutter_hooks/flutter_hooks.dart';
import 'package:typed_data/typed_buffers.dart';
import 'package:url_launcher/url_launcher.dart';
import 'package:usb_serial/usb_serial.dart';
import 'package:wellship_serial_client/data/provider/behavior_settings_provider.dart';
import 'package:wellship_serial_client/data/provider/wired_devices_provider.dart';
import 'package:wellship_serial_client/data/provider/wired_settings_provider.dart';
import 'package:wellship_serial_client/ui/component/wsc_control_char_escaped_selectable_text.dart';

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
        port.inputStream!.listen(
          (data) async {
            buffer.addAll(data);
            text.value = utf8.decode(buffer);
            // Ack判定
            if (behaviorSettings.shouldAck(buffer, data)) {
              port.write(utf8.encode(behaviorSettings.ackString ?? ''));
            }
            // 停止判定
            if (!aborting && behaviorSettings.shouldAbort(buffer)) {
              aborting = true;
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
      }).onError((x, s) async {
        error.value = x.toString();
      });
      return null;
    }, [firstDevice]);
    return Scaffold(
      appBar: AppBar(
        title: Text(behaviorSettings.title ?? '有線接続通信画面'),
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
                ListTile(title: Text(firstDevice?.deviceName ?? ''), onTap: null),
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
                        usbPort.value != null ? "準備完了！" : "準備中",
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
              child: WscControlCharEscapedSelectableText(
                text.value,
              ),
            ),
          ),
        ],
      ),
    );
  }
}
