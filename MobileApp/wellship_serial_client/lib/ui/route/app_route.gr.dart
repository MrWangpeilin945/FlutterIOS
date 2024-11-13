// GENERATED CODE - DO NOT MODIFY BY HAND

// **************************************************************************
// AutoRouterGenerator
// **************************************************************************

// ignore_for_file: type=lint
// coverage:ignore-file

// ignore_for_file: no_leading_underscores_for_library_prefixes
import 'package:auto_route/auto_route.dart' as _i5;
import 'package:wellship_serial_client/ui/page/deep_link_home/deep_link_home_page.dart'
    as _i1;
import 'package:wellship_serial_client/ui/page/home/home_page.dart' as _i2;
import 'package:wellship_serial_client/ui/page/wired_serial/wired_serial_communication_page.dart'
    as _i3;
import 'package:wellship_serial_client/ui/page/wired_serial/wired_serial_settings_page.dart'
    as _i4;

/// generated route for
/// [_i1.DeepLinkHomePage]
class DeepLinkHomeRoute extends _i5.PageRouteInfo<void> {
  const DeepLinkHomeRoute({List<_i5.PageRouteInfo>? children})
      : super(
          DeepLinkHomeRoute.name,
          initialChildren: children,
        );

  static const String name = 'DeepLinkHomeRoute';

  static _i5.PageInfo page = _i5.PageInfo(
    name,
    builder: (data) {
      return const _i1.DeepLinkHomePage();
    },
  );
}

/// generated route for
/// [_i2.HomePage]
class HomeRoute extends _i5.PageRouteInfo<void> {
  const HomeRoute({List<_i5.PageRouteInfo>? children})
      : super(
          HomeRoute.name,
          initialChildren: children,
        );

  static const String name = 'HomeRoute';

  static _i5.PageInfo page = _i5.PageInfo(
    name,
    builder: (data) {
      return const _i2.HomePage();
    },
  );
}

/// generated route for
/// [_i3.WiredSerialCommunicationPage]
class WiredSerialCommunicationRoute extends _i5.PageRouteInfo<void> {
  const WiredSerialCommunicationRoute({List<_i5.PageRouteInfo>? children})
      : super(
          WiredSerialCommunicationRoute.name,
          initialChildren: children,
        );

  static const String name = 'WiredSerialCommunicationRoute';

  static _i5.PageInfo page = _i5.PageInfo(
    name,
    builder: (data) {
      return const _i3.WiredSerialCommunicationPage();
    },
  );
}

/// generated route for
/// [_i4.WiredSerialSettingsPage]
class WiredSerialSettingsRoute extends _i5.PageRouteInfo<void> {
  const WiredSerialSettingsRoute({List<_i5.PageRouteInfo>? children})
      : super(
          WiredSerialSettingsRoute.name,
          initialChildren: children,
        );

  static const String name = 'WiredSerialSettingsRoute';

  static _i5.PageInfo page = _i5.PageInfo(
    name,
    builder: (data) {
      return const _i4.WiredSerialSettingsPage();
    },
  );
}
