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
                      text: '测试蓝牙状态',
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
      debugPrint('显示蓝牙测试对话框');

      // 调用 SerialPlatform 的方法来检查蓝牙是否开启
      debugPrint('正在检查蓝牙是否开启...');
      final isBluetoothEnabled = await SerialPlatform.isBluetoothEnabled();
      debugPrint('蓝牙状态检查完成，蓝牙是否开启: $isBluetoothEnabled'); // 打印蓝牙的状态

      // 弹出对话框，显示蓝牙是否开启
      showDialog(
        // ignore: use_build_context_synchronously
        context: context,
        builder: (BuildContext context) {
          return AlertDialog(
            title: const Text('蓝牙功能测试结果'), // 这里是更新后的标题
            content: Text(isBluetoothEnabled ? '蓝牙已开启' : '蓝牙未开启'), // 更新提示内容
            actions: <Widget>[
              TextButton(
                onPressed: () {
                  Navigator.of(context).pop();
                  debugPrint('关闭蓝牙测试对话框'); // 按钮点击时输出日志
                },
                child: const Text('关闭'),
              ),
            ],
          );
        },
      );
    } on Exception catch (e) {
      debugPrint('检查蓝牙状态失败: $e');
      // 处理异常，弹出错误信息
      showDialog(
        // ignore: use_build_context_synchronously
        context: context,
        builder: (BuildContext context) {
          return AlertDialog(
            title: const Text('错误'), // 这里是错误对话框的标题
            content: Text('检查蓝牙状态失败: $e'), // 错误信息内容
            actions: <Widget>[
              TextButton(
                onPressed: () {
                  Navigator.of(context).pop();
                  debugPrint('关闭错误对话框');
                },
                child: const Text('关闭'),
              ),
            ],
          );
        },
      );
    }
  }
}
