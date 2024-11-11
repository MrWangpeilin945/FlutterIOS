import 'package:auto_route/auto_route.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:wellship_serial_client/ui/route/app_route.gr.dart';

final routerProvider = Provider((ref) => AppRouter());

@AutoRouterConfig(replaceInRouteName: 'Screen|Page,Route')
class AppRouter extends RootStackRouter {
  @override
  RouteType get defaultRouteType => const RouteType.material();

  @override
  List<AutoRoute> get routes => [
        AutoRoute(path: '/', page: HomeRoute.page),
        AutoRoute(page: WiredSerialSettingsRoute.page),
        AutoRoute(path: '/deep-link-home', page: DeepLinkHomeRoute.page),
      ];
}
