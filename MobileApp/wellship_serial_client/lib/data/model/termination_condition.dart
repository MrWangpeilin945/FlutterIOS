import 'package:freezed_annotation/freezed_annotation.dart';
import 'package:wellship_serial_client/data/enum/parity.dart';
import 'package:wellship_serial_client/data/enum/stop_bits.dart';
import 'package:wellship_serial_client/data/enum/trans_method.dart';

part 'termination_condition.freezed.dart';

@freezed
class TerminationCondition with _$TerminationCondition {
  const factory TerminationCondition(
      {required TransMethod transMethod,
      String? title,
      String? callback,
      @Default(9600) int baud,
      @Default(8) int dataBits,
      @Default(Parity.none) Parity parity,
      @Default(StopBits.stopBits_1) stopBits,
      List<String>? ackTriggerStrings,
      String? ackString,
      String? eotString,
      int? dataLength,
      @Default(false) bool useRts,
      @Default(false) bool useDtr,
      String? transmissionDataTriggerString,
      String? transmissionData,
      @Default(false) bool debugMode}) = _TerminationCondition;

  factory TerminationCondition.fromMap(Map<String, dynamic> map) {
    return TerminationCondition(
        transMethod: TransMethod.fromString(map['transMethod']),
        title: map['title'] ?? '',
        callback: map['callback'],
        baud: int.tryParse(map['baud'] ?? '') ?? 9600,
        dataBits: int.tryParse(map['dataBits'] ?? '') ?? 8,
        parity: Parity.fromString(map['parity'] ?? ''),
        stopBits: StopBits.fromString(map['stopBits'] ?? ''),
        ackTriggerStrings: (map['ackTriggerStrings'] ?? '').split(','),
        ackString: map['ackString'] ?? '',
        eotString: map['eotString'] ?? '',
        dataLength: int.tryParse(map['dataLength'] ?? ''),
        useRts: bool.tryParse(map['useRts'] ?? '') ?? false,
        useDtr: bool.tryParse(map['useDtr'] ?? '') ?? false,
        transmissionDataTriggerString: map['transmissionDataTriggerString'] ?? '',
        transmissionData: map['transmissionData'] ?? '',
        debugMode: bool.tryParse(map['debugMode'] ?? '') ?? false);
  }
}
