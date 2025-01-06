import 'package:hooks_riverpod/hooks_riverpod.dart';
import 'package:wellship_serial_client/data/model/bt_classic_settings.dart';

final btClassicSettingsProvider = StateProvider<BtClassicSettings>((ref) => const BtClassicSettings());
