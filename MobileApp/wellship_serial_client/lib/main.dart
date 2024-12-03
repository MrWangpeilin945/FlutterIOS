import 'package:flutter/material.dart';
import 'package:flutter_localizations/flutter_localizations.dart';
import 'package:hooks_riverpod/hooks_riverpod.dart';
import 'package:wellship_serial_client/ui/route/app_route.dart';

void main() {
  runApp(const ProviderScope(child: App()));
}

class App extends ConsumerWidget {
  const App({super.key});

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final router = ref.watch(routerProvider);
    return MaterialApp.router(
      title: 'WELLSHIP Serial Client',
      theme: ThemeData(
        colorScheme: ColorScheme.fromSeed(seedColor: Colors.teal),
        textTheme: const TextTheme(
          displayLarge: TextStyle(),
          displayMedium: TextStyle(),
          displaySmall: TextStyle(),
          headlineLarge: TextStyle(),
          headlineMedium: TextStyle(),
          headlineSmall: TextStyle(),
          titleLarge: TextStyle(fontSize: 33),
          titleMedium: TextStyle(fontSize: 24),
          titleSmall: TextStyle(fontSize: 21),
          bodyLarge: TextStyle(fontSize: 24),
          bodyMedium: TextStyle(fontSize: 21),
          bodySmall: TextStyle(fontSize: 18),
          labelLarge: TextStyle(fontSize: 21),
          labelMedium: TextStyle(fontSize: 18),
          labelSmall: TextStyle(fontSize: 16.5),
        ),
        useMaterial3: true,
      ),
      routerConfig: router.config(),
      supportedLocales: const [Locale('ja', 'JP')],
      locale: const Locale('ja', 'JP'),
      restorationScopeId: 'wellship_serial_client',
      localizationsDelegates: const {
        GlobalMaterialLocalizations.delegate,
        GlobalCupertinoLocalizations.delegate,
        GlobalWidgetsLocalizations.delegate
      },
    );
  }
}
