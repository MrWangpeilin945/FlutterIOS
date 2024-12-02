import 'package:freezed_annotation/freezed_annotation.dart';
import 'package:hooks_riverpod/hooks_riverpod.dart';
import 'package:wellship_serial_client/data/enum/parity.dart';
import 'package:wellship_serial_client/data/enum/stop_bits.dart';

part 'wired_settings.freezed.dart';

final wiredSettingsProvider = StateProvider<WiredSettings>((ref) => const WiredSettings(
      baud: 9600,
      dataBits: 8,
      parity: Parity.none,
      stopBits: StopBits.stopBits_1,
      useRts: false,
      useDtr: false,
    ));

@freezed
abstract class WiredSettings implements _$WiredSettings {
  const factory WiredSettings(
      {required int baud,
      required int dataBits,
      required Parity parity,
      required StopBits stopBits,
      required bool useRts,
      required bool useDtr}) = _WiredSettings;

  // Uri? callbackUri{
  //   final callbackUri = Uri.tryParse(callback);
  // }
}
