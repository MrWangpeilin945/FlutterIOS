import 'package:freezed_annotation/freezed_annotation.dart';
import 'package:wellship_serial_client/data/enum/parity.dart';
import 'package:wellship_serial_client/data/enum/stop_bits.dart';
import 'package:wellship_serial_client/data/enum/trans_method.dart';

part 'query_parameter.freezed.dart';

@freezed
class QueryParameter with _$QueryParameter {
  const factory QueryParameter({
    required TransMethod transMethod,
    String? title,
    String? callback,
    int? baud,
    int? dataBits,
    Parity? parity,
    StopBits? stopBits,
    List<String>? ackTriggerStrings,
    String? ackString,
    String? eotString,
    int? dataLength,
    bool? useRts,
    bool? useDtr,
    String? transmissionDataTriggerString,
    String? transmissionData,
  }) = _QueryParameter;

  factory QueryParameter.fromMap(Map<String, dynamic> map) {
    return QueryParameter(
        transMethod: TransMethod.fromString(map['transMethod']),
        title: map['title'] ?? '',
        callback: map['callback'],
        baud: int.tryParse(map['baud'] ?? ''),
        dataBits: int.tryParse(map['dataBits'] ?? ''),
        parity: Parity.fromValue(int.tryParse(map['parity'] ?? '') ?? 0),
        stopBits: StopBits.fromValue(int.tryParse(map['stopBits'] ?? '') ?? 0),
        ackTriggerStrings: (map['ackTriggerStrings'] ?? '').split(','),
        ackString: map['ackString'] ?? '',
        eotString: map['eotString'] ?? '',
        dataLength: int.tryParse(map['dataLength'] ?? ''),
        useRts: bool.tryParse(map['useRts'] ?? ''),
        useDtr: bool.tryParse(map['useDtr'] ?? ''),
        transmissionDataTriggerString: map['transmissionDataTriggerString'] ?? '',
        transmissionData: map['transmissionData'] ?? '');
  }
}
