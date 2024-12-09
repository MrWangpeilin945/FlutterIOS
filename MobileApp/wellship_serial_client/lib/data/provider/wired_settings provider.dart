import 'package:hooks_riverpod/hooks_riverpod.dart';
import 'package:wellship_serial_client/data/enum/parity.dart';
import 'package:wellship_serial_client/data/enum/stop_bits.dart';
import 'package:wellship_serial_client/data/model/wired_settings.dart';

final wiredSettingsProvider = StateProvider<WiredSettings>((ref) => const WiredSettings(
      baud: 9600,
      dataBits: 8,
      parity: Parity.none,
      stopBits: StopBits.stopBits_1,
      useRts: false,
      useDtr: false,
    ));
