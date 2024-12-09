import 'package:freezed_annotation/freezed_annotation.dart';

part 'bt_classic_settings.freezed.dart';

@freezed
class BtClassicSettings with _$BtClassicSettings {
  const factory BtClassicSettings({
    String? deviceName,
    String? address,
  }) = _BtClassicSettings;
}
