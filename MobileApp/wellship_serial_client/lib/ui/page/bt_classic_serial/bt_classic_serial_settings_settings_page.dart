import 'package:auto_route/auto_route.dart';
import 'package:flutter/material.dart';
import 'package:flutter_bluetooth_serial/flutter_bluetooth_serial.dart';
import 'package:flutter_hooks/flutter_hooks.dart';
import 'package:hooks_riverpod/hooks_riverpod.dart';
import 'package:wellship_serial_client/data/model/bt_classic_settings.dart';
import 'package:wellship_serial_client/data/provider/bt_classic_devices_provider.dart';
import 'package:wellship_serial_client/data/provider/bt_classic_settings_provider.dart';
import 'package:wellship_serial_client/ui/component/wsc_select_dialog.dart';
import 'package:wellship_serial_client/ui/route/app_route.dart';
import 'package:wellship_serial_client/ui/route/app_route.gr.dart';

@RoutePage()
class BtClassicSerialSettingsPage extends HookConsumerWidget {
  const BtClassicSerialSettingsPage({super.key});

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final deviceStream = ref.watch(btClassicDevicesProvider);
    final router = ref.read(routerProvider);

    final device = useState<BluetoothDevice?>(null);

    return Scaffold(
      appBar: AppBar(
        title: const Text('BluetoothClassic接続'),
        backgroundColor: Theme.of(context).colorScheme.inversePrimary,
      ),
      body: SingleChildScrollView(
        child: Center(
          child: Padding(
            padding: const EdgeInsets.all(16.0),
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.stretch,
              mainAxisAlignment: MainAxisAlignment.start,
              children: [
                ListTile(
                    leading: const Icon(Icons.cable),
                    title: const Text("接続設定"),
                    textColor: Theme.of(context).colorScheme.primary),
                ListTile(
                  title: const Text("接続デバイス"),
                  subtitle: Text('${device.value?.name}'),
                  trailing: Row(
                    mainAxisSize: MainAxisSize.min,
                    children: [
                      Text('${deviceStream.value?.length ?? 0}件のデバイスを検出'),
                      IconButton(
                          onPressed: () => ref.refresh(btClassicDevicesProvider),
                          icon: const Icon(Icons.replay_outlined))
                    ],
                  ),
                  onTap: deviceStream.hasValue
                      ? () async {
                          await showDialog(
                            context: context,
                            builder: (context) {
                              return WscSelectDialog(
                                title: "接続デバイスを選択してください",
                                items: deviceStream.value?.map((x) => x.device).toList() ?? <BluetoothDevice>[],
                                mapper: (x) => RadioListTile(
                                  value: x,
                                  title: Text(x.name ?? ''),
                                  subtitle: Text(x.address),
                                  groupValue: device.value,
                                  onChanged: (value) {
                                    device.value = value;
                                    Navigator.pop(context);
                                  },
                                ),
                              );
                            },
                          );
                        }
                      : null,
                ),
                ListTile(
                    leading: const Icon(Icons.swap_horiz),
                    title: const Text("通信設定"),
                    textColor: Theme.of(context).colorScheme.primary),
                ListTile(
                  title: const Text("Ack文字列"),
                  subtitle: Text(String.fromCharCode(0x2406)),
                  onTap: () {},
                ),
                ListTile(
                  title: const Text("Ack送信条件文字列"),
                  subtitle: Text("${String.fromCharCode(0x2403)} / ${String.fromCharCode(0x240d)}"),
                  onTap: () {},
                ),
                ListTile(
                  title: const Text("終了条件文字列"),
                  subtitle: Text(String.fromCharCode(0x2404)),
                  onTap: () {},
                ),
                ListTile(
                  title: const Text("終了条件バイト数"),
                  subtitle: const Text("128"),
                  onTap: () {},
                ),
              ],
            ),
          ),
        ),
      ),
      bottomNavigationBar: Padding(
        padding: const EdgeInsets.fromLTRB(16, 0, 16, 32),
        child: FilledButton(
            onPressed: () async {
              final selectedDevice = device.value;
              if (selectedDevice == null) {
                return;
              }
              ref.read(btClassicSettingsProvider.notifier).state =
                  BtClassicSettings(deviceName: selectedDevice.name, address: selectedDevice.address);
              router.push(const BtClassicSerialCommunicationRoute());
            },
            child: const Text('読み取り開始')),
      ),
    );
  }
}
