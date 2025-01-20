import 'package:freezed_annotation/freezed_annotation.dart';
import 'package:wellship_serial_client/data/enum/parity.dart';
import 'package:wellship_serial_client/data/enum/stop_bits.dart';
import 'package:wellship_serial_client/data/enum/trans_method.dart';
import 'package:wellship_serial_client/data/model/behavior_settings.dart';
import 'package:wellship_serial_client/data/model/wired_settings.dart';

part 'app_settings.freezed.dart';

@freezed
abstract class AppSettings implements _$AppSettings {
  const AppSettings._();
  const factory AppSettings(
      {@Default(TransMethod.none) TransMethod transMethod,
      String? title,
      String? callback,
      @Default(9600) int baud,
      @Default(8) int dataBits,
      @Default(Parity.none) Parity parity,
      @Default(StopBits.stopBits_1) StopBits stopBits,
      List<String>? ackTriggers,
      String? ackString,
      String? eotString,
      int? dataLength,
      @Default(false) bool useRts,
      @Default(false) bool useDtr,
      String? transmissionDataTrigger,
      String? transmissionData,
      @Default(false) bool debugMode}) = _AppSettings;

  factory AppSettings.fromMap(Map<String, dynamic> map) {
    return AppSettings(
        transMethod: TransMethod.fromString(map['transMethod']),
        title: map['title'] ?? '',
        callback: map['callback'],
        baud: int.tryParse(map['baud'] ?? '') ?? 9600,
        dataBits: int.tryParse(map['dataBits'] ?? '') ?? 8,
        parity: Parity.fromString(map['parity'] ?? ''),
        stopBits: StopBits.fromString(map['stopBits'] ?? ''),
        ackTriggers: (map['ackTriggers'] ?? '').split(','),
        ackString: map['ackString'] ?? '',
        eotString: map['eotString'] ?? '',
        dataLength: int.tryParse(map['dataLength'] ?? ''),
        useRts: bool.tryParse(map['useRts'] ?? '') ?? false,
        useDtr: bool.tryParse(map['useDtr'] ?? '') ?? false,
        transmissionDataTrigger: map['transDataTrigger'] ?? '',
        transmissionData: map['transData'] ?? '',
        debugMode: bool.tryParse(map['debugMode'] ?? '') ?? false);
  }
  WiredSettings toWiredSettings() => WiredSettings(
        baud: baud,
        dataBits: dataBits,
        parity: parity,
        stopBits: stopBits,
        useRts: useRts,
        useDtr: useDtr,
      );

  BehaviorSettings toBehaviorSettings() {
    final callbackUri = Uri.tryParse(callback ?? '');
    return BehaviorSettings(
      ackString: ackString,
      ackTriggers: ackTriggers,
      callback: callbackUri,
      dataLength: dataLength,
      eotString: eotString,
      title: title,
      transmissionData: transmissionData,
      transmissionDataTrigger: transmissionDataTrigger,
    );
  }
}
