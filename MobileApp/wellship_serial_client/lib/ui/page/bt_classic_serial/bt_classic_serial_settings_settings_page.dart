import 'package:auto_route/auto_route.dart';
import 'package:flutter/material.dart';
import 'package:hooks_riverpod/hooks_riverpod.dart';
import 'package:permission_handler/permission_handler.dart';
import 'package:wellship_serial_client/data/handler/bluetooth_permission_handler.dart';
import 'package:wellship_serial_client/data/handler/location_permission_handler.dart';
import 'package:wellship_serial_client/data/provider/bt_classic_settings.dart';
import 'package:wellship_serial_client/ui/route/app_route.dart';
import 'package:wellship_serial_client/ui/route/app_route.gr.dart';

@RoutePage()
class BtClassicSerialSettingsPage extends ConsumerWidget {
  const BtClassicSerialSettingsPage({super.key});

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final router = ref.read(routerProvider);
    return Scaffold(
      appBar: AppBar(
        title: const Text('BluetoothClassic接続'),
        backgroundColor: Theme.of(context).colorScheme.inversePrimary,
      ),
      body: LayoutBuilder(
        builder: (context, constraints) => SingleChildScrollView(
          child: ConstrainedBox(
            constraints: BoxConstraints(maxHeight: constraints.maxHeight),
            child: Center(
              child: Padding(
                padding: const EdgeInsets.all(16.0),
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.stretch,
                  mainAxisAlignment: MainAxisAlignment.center,
                  children: [
                    OutlinedButton(
                        onPressed: () async {
                          var result = await _requestPermission();
                          if (!result) {
                            showDialog(
                                // TODO fix!
                                context: context,
                                builder: (context) => const AlertDialog(
                                      content: Text('必要な権限が与えられていないため実行できません。\r\n設定画面から位置情報・付近のデバイスを検出する権限を付与してください。'),
                                    ));
                            return;
                          }
                          ref.read(btClassicSettingsProvider.notifier).state = const BtClassicSettings();
                          router.push(const BtClassicSerialCommunicationRoute());
                        },
                        child: const Text('読み取り開始'))
                  ],
                ),
              ),
            ),
          ),
        ),
      ),
    );
  }

  Future<bool> _requestPermission() async {
    final locationPermissionHandler = LocationPermissionsHandler();
    final bluetoothPermissionHandler = BluetoothPermissionsHandler();
    final locationPermission = await locationPermissionHandler.request();
    final bluetoothPermission = await bluetoothPermissionHandler.request();
    return locationPermission == PermissionStatus.granted && bluetoothPermission == PermissionStatus.granted;
  }
}
