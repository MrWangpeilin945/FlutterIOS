import 'package:freezed_annotation/freezed_annotation.dart';

part 'behavior_settings.freezed.dart';

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
