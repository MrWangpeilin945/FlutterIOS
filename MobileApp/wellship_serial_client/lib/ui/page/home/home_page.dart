import 'package:auto_route/auto_route.dart';
import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
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
      body: Padding(
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
              onPressed: () => {router.push(const WiredSerialSettingsRoute())},
              text: '無線接続（BR/EDR）',
            ),
            WscMenuButton(
              onPressed: () => {router.push(const WiredSerialSettingsRoute())},
              text: '無線接続 (BLE)',
            ),
          ],
        ),
      ),
    );
  }
}
