import 'dart:math';

import 'package:auto_route/auto_route.dart';
import 'package:flutter/material.dart';
import 'package:hooks_riverpod/hooks_riverpod.dart';
import 'package:url_launcher/url_launcher.dart';
import 'package:wellship_serial_client/data/enum/trans_method.dart';
import 'package:wellship_serial_client/data/model/app_settings.dart';
import 'package:wellship_serial_client/data/provider/bt_classic_settings.dart';
import 'package:wellship_serial_client/data/provider/wired_settings.dart';
import 'package:wellship_serial_client/ui/route/app_route.gr.dart';

@RoutePage()
class DeepLinkHomePage extends ConsumerWidget {
  const DeepLinkHomePage({super.key});

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final router = context.router;
    final params = router.current.queryParams;
    final settings = AppSettings.fromMap(params.rawMap);
    switch (settings.transMethod) {
      case TransMethod.wired:
        // Buildメソッド内でStateProviderを更新してはならないため
        WidgetsBinding.instance.addPostFrameCallback((_) {
          ref.read(wiredSettingsProvider.notifier).state = WiredSettings(
            baud: settings.baud,
            dataBits: settings.dataBits,
            parity: settings.parity,
            stopBits: settings.stopBits,
            useRts: settings.useRts,
            useDtr: settings.useDtr,
          );
        });
        router.replace(const WiredSerialCommunicationRoute());
        return const Scaffold();
      case TransMethod.btClassic:
        WidgetsBinding.instance.addPostFrameCallback((_) {
          ref.read(btClassicSettingsProvider.notifier).state = const BtClassicSettings();
        });
        router.replace(const BtClassicSerialCommunicationRoute());
        return const Scaffold();
      default:
    }

    return Scaffold(
      appBar: AppBar(
        title: const Text('Deep Link'),
        automaticallyImplyLeading: false,
      ),
      body: Padding(
        padding: const EdgeInsets.all(16.0),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.stretch,
          mainAxisAlignment: MainAxisAlignment.center,
          children: [
            const Text('Deep Link'),
            Text(settings.toString()),
            OutlinedButton(
                onPressed: () async {
                  final random = Random();
                  await launchUrl(Uri.parse('http://10.167.2.216/query-receiver.html?value=${random.nextInt(10000)}'),
                      mode: LaunchMode.externalApplication);
                },
                child: const Text('Webに戻る'))
          ],
        ),
      ),
    );
  }
}
