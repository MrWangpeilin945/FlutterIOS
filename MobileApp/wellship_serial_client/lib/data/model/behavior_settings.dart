import 'dart:convert';
import 'dart:typed_data';

import 'package:freezed_annotation/freezed_annotation.dart';
import 'package:typed_data/typed_buffers.dart';

part 'behavior_settings.freezed.dart';

@freezed
abstract class BehaviorSettings implements _$BehaviorSettings {
  const BehaviorSettings._();
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

  bool shouldAck(Uint8Buffer buffer, Uint8List data) {
    if (ackString == null || (ackTriggers?.isEmpty ?? true)) {
      return false;
    }
    // TODO: Ackを返す判定を実装する
    return false;
  }

  bool shouldAbort(Uint8Buffer buffer) {
    if (dataLength != null && buffer.length >= dataLength!) {
      return true;
    }
    if (eotString != null && utf8.decode(buffer).contains(eotString!)) {
      return true;
    }
    return false;
  }
}
