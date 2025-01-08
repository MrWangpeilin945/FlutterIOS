import 'package:auto_route/auto_route.dart';
import 'package:flutter/material.dart';
import 'package:hooks_riverpod/hooks_riverpod.dart';
import 'package:permission_handler/permission_handler.dart';
import 'package:wellship_serial_client/data/handler/bluetooth_permission_handler.dart';
import 'package:wellship_serial_client/data/handler/location_permission_handler.dart';
import 'package:wellship_serial_client/ui/component/wsc_menu_button.dart';
import 'package:wellship_serial_client/ui/route/app_route.dart';
import 'package:wellship_serial_client/ui/route/app_route.gr.dart';

@RoutePage()
class HomePage extends ConsumerWidget {
  const HomePage({super.key});

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final router = ref.read(routerProvider);
    return Scaffold(
      appBar: AppBar(
        title: const Text('WELLSHIP Serial Client'),
        backgroundColor: Theme.of(context).colorScheme.inversePrimary,
      ),
      body: LayoutBuilder(
        builder: (context, constraints) => ConstrainedBox(
          constraints: BoxConstraints(maxHeight: constraints.maxHeight),
          child: SingleChildScrollView(
            child: Center(
              child: Padding(
                padding: const EdgeInsets.all(16.0),
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.stretch,
                  mainAxisAlignment: MainAxisAlignment.center,
                  children: [
                    WscMenuButton(
                      onPressed: () => {router.push(const WiredSerialSettingsRoute())},
                      text: '有線接続',
                    ),
                    WscMenuButton(
                      onPressed: () async {
                        final result = await _requestPermission();
                        if (!result && context.mounted) {
                          showDialog(
                              context: context,
                              builder: (context) => const AlertDialog(
                                    content: Text('必要な権限が与えられていないため実行できません。\r\n設定画面から位置情報・付近のデバイスを検出する権限を付与してください。'),
                                  ));
                          return;
                        }
                        router.push(const BtClassicSerialSettingsRoute());
                      },
                      text: '無線接続（BR/EDR）',
                    ),
                    const WscMenuButton(
                      onPressed: null,
                      text: '無線接続 (BLE)',
                    ),
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
    final bluetoothScanPermission = await bluetoothPermissionHandler.requestScan();
    final bluetoothConnectPermission = await bluetoothPermissionHandler.requestConnect();
    return locationPermission == PermissionStatus.granted &&
        bluetoothScanPermission == PermissionStatus.granted &&
        bluetoothConnectPermission == PermissionStatus.granted;
  }
}
