import 'package:freezed_annotation/freezed_annotation.dart';

part 'termination_condition.freezed.dart';

@freezed
class TerminationCondition with _$TerminationCondition {
  const factory TerminationCondition({
    @Default([]) List<String> ackTriggers,
    String? ackString,
    String? eotString,
    int? dataLength,
  }) = _TerminationCondition;
}
