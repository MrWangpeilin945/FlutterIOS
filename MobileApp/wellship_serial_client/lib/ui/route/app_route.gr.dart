// GENERATED CODE - DO NOT MODIFY BY HAND

// **************************************************************************
// AutoRouterGenerator
// **************************************************************************

// ignore_for_file: type=lint
// coverage:ignore-file

// ignore_for_file: no_leading_underscores_for_library_prefixes
import 'package:auto_route/auto_route.dart' as _i3;
import 'package:wellship_serial_client/ui/page/deep_link_home/deep_link_home_page.dart'
    as _i1;
import 'package:wellship_serial_client/ui/page/home/home_page.dart' as _i2;

/// generated route for
/// [_i1.DeepLinkHomePage]
class DeepLinkHomeRoute extends _i3.PageRouteInfo<void> {
  const DeepLinkHomeRoute({List<_i3.PageRouteInfo>? children})
      : super(
          DeepLinkHomeRoute.name,
          initialChildren: children,
        );

  static const String name = 'DeepLinkHomeRoute';

  static _i3.PageInfo page = _i3.PageInfo(
    name,
    builder: (data) {
      return const _i1.DeepLinkHomePage();
    },
  );
}

/// generated route for
/// [_i2.HomePage]
class HomeRoute extends _i3.PageRouteInfo<void> {
  const HomeRoute({List<_i3.PageRouteInfo>? children})
      : super(
          HomeRoute.name,
          initialChildren: children,
        );

  static const String name = 'HomeRoute';

  static _i3.PageInfo page = _i3.PageInfo(
    name,
    builder: (data) {
      return const _i2.HomePage();
    },
  );
}
