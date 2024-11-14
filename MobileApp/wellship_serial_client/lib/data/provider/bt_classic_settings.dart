import 'package:freezed_annotation/freezed_annotation.dart';
import 'package:hooks_riverpod/hooks_riverpod.dart';

part 'bt_classic_settings.freezed.dart';

final btClassicSettingsProvider = StateProvider<BtClassicSettings>((ref) => const BtClassicSettings());

@freezed
class BtClassicSettings with _$BtClassicSettings {
  const factory BtClassicSettings(
      {List<String>? ackTriggerStrings,
      String? ackString,
      String? eotString,
      int? dataLength,
      String? transmissionDataTriggerString,
      String? transmissionData}) = _BtClassicSettings;
}
