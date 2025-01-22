import 'package:freezed_annotation/freezed_annotation.dart';
import 'package:wellship_serial_client/data/enum/parity.dart';
import 'package:wellship_serial_client/data/enum/stop_bits.dart';

part 'wired_settings.freezed.dart';

@freezed
class WiredSettings with _$WiredSettings {
  const factory WiredSettings(
      {required int baud,
      required int dataBits,
      required Parity parity,
      required StopBits stopBits,
      required bool useRts,
      required bool useDtr}) = _WiredSettings;
}
