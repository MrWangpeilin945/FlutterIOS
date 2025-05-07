import 'package:auto_route/auto_route.dart';
import 'package:flutter/material.dart';
import 'package:hooks_riverpod/hooks_riverpod.dart';
import 'package:permission_handler/permission_handler.dart';
import 'package:wellship_serial_client/data/handler/bluetooth_permission_handler.dart';
import 'package:wellship_serial_client/data/handler/location_permission_handler.dart';
import 'package:wellship_serial_client/data/serial/serial_platform.dart';
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
                      onPressed: () =>
                          {router.push(const WiredSerialSettingsRoute())},
                      text: '有線接続',
                    ),
                    WscMenuButton(
                      onPressed: () async {
                        final result = await _requestPermission();
                        if (!result && context.mounted) {
                          showDialog(
                              context: context,
                              builder: (context) => const AlertDialog(
                                    content: Text(
                                        '必要な権限が与えられていないため実行できません。\r\n設定画面から位置情報・付近のデバイスを検出する権限を付与してください。'),
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
                    WscMenuButton(
                      onPressed: () => _showBluetoothTestDialog(context),
                      text: 'Bluetooth状態をチェック',
                    ),
                    // iOS端のみBluetoothスキャンボタンを表示
                    if (PlatformUtils.isIOS)
                      WscMenuButton(
                        onPressed: () => _scanBluetoothDevices(context),
                        text: 'Bluetoothスキャン（iOS）',
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
    final bluetoothScanPermission =
        await bluetoothPermissionHandler.requestScan();
    final bluetoothConnectPermission =
        await bluetoothPermissionHandler.requestConnect();
    return locationPermission == PermissionStatus.granted &&
        bluetoothScanPermission == PermissionStatus.granted &&
        bluetoothConnectPermission == PermissionStatus.granted;
  }

  Future<void> _showBluetoothTestDialog(BuildContext context) async {
    try {
      debugPrint('ブルートゥーステストダイアログを表示する');

      // SerialPlatform のメソッドを呼び出して、ブルートゥースがオンになっているかどうかを確認する。
      debugPrint('ブルートゥースがオンになっているかどうかを確認中...');
      final isBluetoothEnabled = await SerialPlatform.isBluetoothEnabled();
      debugPrint('ブルートゥースのオン状態： $isBluetoothEnabled');

      showDialog(
        // ignore: use_build_context_synchronously
        context: context,
        builder: (BuildContext context) {
          return AlertDialog(
            title: const Text('Bluetoothの状態'),
            content:
                Text(isBluetoothEnabled ? 'Bluetoothが有効です' : 'Bluetoothが無効です'),
            actions: <Widget>[
              TextButton(
                onPressed: () {
                  Navigator.of(context).pop();
                  debugPrint('Bluetoothテストダイアログを閉じる');
                },
                child: const Text('閉じる'),
              ),
            ],
          );
        },
      );
    } on Exception catch (e) {
      debugPrint('Bluetoothの状態確認に失敗しました: $e');
      showDialog(
        // ignore: use_build_context_synchronously
        context: context,
        builder: (BuildContext context) {
          return AlertDialog(
            title: const Text('エラー'),
            content: Text('Bluetoothの状態確認に失敗しました: $e'),
            actions: <Widget>[
              TextButton(
                onPressed: () {
                  Navigator.of(context).pop();
                  debugPrint('エラーダイアログを閉じる');
                },
                child: const Text('閉じる'),
              ),
            ],
          );
        },
      );
    }
  }

// iOSのBluetoothデバイススキャン方法
  Future<void> _scanBluetoothDevices(BuildContext context) async {
    try {
      debugPrint('Bluetoothデバイスのスキャンを開始しています…');
      showDialog(
        context: context,
        barrierDismissible: false,
        builder: (context) => const AlertDialog(
          title: Text('スキャン中…'),
          content: Column(
            mainAxisSize: MainAxisSize.min,
            children: [
              CircularProgressIndicator(),
              SizedBox(height: 16),
              Text('近くのBluetoothデバイスを検索中…'),
            ],
          ),
        ),
      );

      final devices = await SerialPlatform.scanForDevices();
      debugPrint('スキャン完了，デバイスを発見: ${devices.length}个');

      // ignore: use_build_context_synchronously
      Navigator.of(context).pop();
      showDialog(
        // ignore: use_build_context_synchronously
        context: context,
        builder: (context) => AlertDialog(
          title: const Text('スキャン結果'),
          content: SizedBox(
            width: double.maxFinite,
            child: devices.isEmpty
                ? const Text('Bluetoothデバイスが見つかりませんでした')
                : Column(
                    mainAxisSize: MainAxisSize.min,
                    crossAxisAlignment: CrossAxisAlignment.start,
                    children: [
                      const Text('発見されたBluetoothデバイス：'),
                      const SizedBox(height: 8),
                      ...devices.map((device) => Text('• $device')),
                    ],
                  ),
          ),
          actions: [
            TextButton(
              onPressed: () => Navigator.of(context).pop(),
              child: const Text('閉じる'),
            ),
          ],
        ),
      );
    } catch (e) {
      debugPrint('Bluetoothデバイスのスキャンに失敗しました: $e');
      if (context.mounted) {
        Navigator.of(context).pop();
        showDialog(
          context: context,
          builder: (context) => AlertDialog(
            title: const Text('エラー'),
            content: Text('Bluetoothデバイスのスキャンに失敗しました: $e'),
            actions: [
              TextButton(
                onPressed: () => Navigator.of(context).pop(),
                child: const Text('閉じる'),
              ),
            ],
          ),
        );
      }
    }
  }
}
