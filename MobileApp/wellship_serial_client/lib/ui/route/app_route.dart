import 'package:auto_route/auto_route.dart';
import 'package:hooks_riverpod/hooks_riverpod.dart';
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
        AutoRoute(page: WiredSerialCommunicationRoute.page),
        AutoRoute(page: BtClassicSerialSettingsRoute.page),
        AutoRoute(page: BtClassicSerialCommunicationRoute.page),
        AutoRoute(path: '/deep-link-home', page: DeepLinkHomeRoute.page),
      ];
}
