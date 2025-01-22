import 'package:auto_route/auto_route.dart';
import 'package:flutter/material.dart';
import 'package:hooks_riverpod/hooks_riverpod.dart';
import 'package:url_launcher/url_launcher.dart';
import 'package:wellship_serial_client/data/enum/trans_method.dart';
import 'package:wellship_serial_client/data/model/app_settings.dart';
import 'package:wellship_serial_client/data/model/bt_classic_settings.dart';
import 'package:wellship_serial_client/data/provider/behavior_settings_provider.dart';
import 'package:wellship_serial_client/data/provider/bt_classic_settings_provider.dart';
import 'package:wellship_serial_client/data/provider/wired_settings_provider.dart';
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
        WidgetsBinding.instance.addPostFrameCallback((_) {
          ref.read(wiredSettingsProvider.notifier).state = settings.toWiredSettings();
          ref.read(behaviorSettingsProvider.notifier).state = settings.toBehaviorSettings();
        });
        router.replace(const WiredSerialCommunicationRoute());
        return const Scaffold();
      case TransMethod.btClassic:
        WidgetsBinding.instance.addPostFrameCallback((_) async {
          ref.read(btClassicSettingsProvider.notifier).state = await BtClassicSettings.loadSettings();
          ref.read(behaviorSettingsProvider.notifier).state = settings.toBehaviorSettings();
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
                onPressed: settings.callback == null
                    ? null
                    : () async {
                        final callback = settings.callback;
                        if (callback == null) {
                          return;
                        }
                        final uri = Uri.tryParse(callback);
                        if (uri == null) {
                          return;
                        }
                        final queryParameters = {"hoge": "fuga"};
                        final hoge = Uri(
                            scheme: uri.scheme,
                            userInfo: uri.userInfo,
                            host: uri.host,
                            port: uri.port,
                            path: uri.path,
                            queryParameters: queryParameters..addAll(uri.queryParameters),
                            fragment: uri.fragment);
                        await launchUrl(hoge, mode: LaunchMode.externalApplication);
                      },
                child: const Text('Webに戻る'))
          ],
        ),
      ),
    );
  }
}
