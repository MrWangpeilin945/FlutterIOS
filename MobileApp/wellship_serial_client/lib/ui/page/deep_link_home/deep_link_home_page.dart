import 'package:auto_route/auto_route.dart';
import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:wellship_serial_client/ui/route/app_route.gr.dart';

@RoutePage()
class DeepLinkHomePage extends ConsumerWidget {
  const DeepLinkHomePage({super.key});

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final router = context.router;
    final params = router.current.queryParams;
    if (params.optString('method') == 'spp') {
      router.push(const HomeRoute());
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
            Text(params.toString()),
          ],
        ),
      ),
    );
  }
}
