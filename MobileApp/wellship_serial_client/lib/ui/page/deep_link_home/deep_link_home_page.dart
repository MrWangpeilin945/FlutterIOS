import 'dart:math';

import 'package:auto_route/auto_route.dart';
import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:url_launcher/url_launcher.dart';
import 'package:wellship_serial_client/data/model/query_parameter.dart';

@RoutePage()
class DeepLinkHomePage extends ConsumerWidget {
  const DeepLinkHomePage({super.key});

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final router = context.router;
    final params = router.current.queryParams;
    final query = QueryParameter.fromMap(params.rawMap);
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
            Text(query.toString()),
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
