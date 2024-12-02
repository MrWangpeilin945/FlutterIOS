import 'package:freezed_annotation/freezed_annotation.dart';
import 'package:hooks_riverpod/hooks_riverpod.dart';

part 'behavior_settings.freezed.dart';

final behaviorSettingsProvider = StateProvider<BehaviorSettings>((ref) => const BehaviorSettings());

@freezed
abstract class BehaviorSettings implements _$BehaviorSettings {
  const factory BehaviorSettings({
    String? title,
    Uri? callback,
    List<String>? ackTriggers,
    String? ackString,
    String? eotString,
    int? dataLength,
    String? transmissionDataTrigger,
    String? transmissionData,
  }) = _BehaviorSettings;
}
