import 'package:auto_route/auto_route.dart';
import 'package:flutter/material.dart';
import 'package:hooks_riverpod/hooks_riverpod.dart';
import 'package:wellship_serial_client/data/enum/parity.dart';
import 'package:wellship_serial_client/data/enum/stop_bits.dart';
import 'package:wellship_serial_client/data/model/wired_settings.dart';
import 'package:wellship_serial_client/ui/route/app_route.dart';
import 'package:wellship_serial_client/ui/route/app_route.gr.dart';

@RoutePage()
class WiredSerialSettingsPage extends ConsumerWidget {
  const WiredSerialSettingsPage({super.key});

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final router = ref.read(routerProvider);
    return Scaffold(
      appBar: AppBar(
        title: const Text('有線接続'),
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
                        onPressed: () {
                          ref.read(wiredSettingsProvider.notifier).state = const WiredSettings(
                              baud: 9600,
                              dataBits: 8,
                              parity: Parity.none,
                              stopBits: StopBits.stopBits_1,
                              useRts: false,
                              useDtr: false);
                          router.push(const WiredSerialCommunicationRoute());
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
}
