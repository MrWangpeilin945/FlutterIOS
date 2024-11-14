// coverage:ignore-file
// GENERATED CODE - DO NOT MODIFY BY HAND
// ignore_for_file: type=lint
// ignore_for_file: unused_element, deprecated_member_use, deprecated_member_use_from_same_package, use_function_type_syntax_for_parameters, unnecessary_const, avoid_init_to_null, invalid_override_different_default_values_named, prefer_expression_function_bodies, annotate_overrides, invalid_annotation_target, unnecessary_question_mark

part of 'bt_classic_settings.dart';

// **************************************************************************
// FreezedGenerator
// **************************************************************************

T _$identity<T>(T value) => value;

final _privateConstructorUsedError = UnsupportedError(
    'It seems like you constructed your class using `MyClass._()`. This constructor is only meant to be used by freezed and you are not supposed to need it nor use it.\nPlease check the documentation here for more information: https://github.com/rrousselGit/freezed#adding-getters-and-methods-to-our-models');

/// @nodoc
mixin _$BtClassicSettings {
  List<String>? get ackTriggerStrings => throw _privateConstructorUsedError;
  String? get ackString => throw _privateConstructorUsedError;
  String? get eotString => throw _privateConstructorUsedError;
  int? get dataLength => throw _privateConstructorUsedError;
  String? get transmissionDataTriggerString =>
      throw _privateConstructorUsedError;
  String? get transmissionData => throw _privateConstructorUsedError;

  /// Create a copy of BtClassicSettings
  /// with the given fields replaced by the non-null parameter values.
  @JsonKey(includeFromJson: false, includeToJson: false)
  $BtClassicSettingsCopyWith<BtClassicSettings> get copyWith =>
      throw _privateConstructorUsedError;
}

/// @nodoc
abstract class $BtClassicSettingsCopyWith<$Res> {
  factory $BtClassicSettingsCopyWith(
          BtClassicSettings value, $Res Function(BtClassicSettings) then) =
      _$BtClassicSettingsCopyWithImpl<$Res, BtClassicSettings>;
  @useResult
  $Res call(
      {List<String>? ackTriggerStrings,
      String? ackString,
      String? eotString,
      int? dataLength,
      String? transmissionDataTriggerString,
      String? transmissionData});
}

/// @nodoc
class _$BtClassicSettingsCopyWithImpl<$Res, $Val extends BtClassicSettings>
    implements $BtClassicSettingsCopyWith<$Res> {
  _$BtClassicSettingsCopyWithImpl(this._value, this._then);

  // ignore: unused_field
  final $Val _value;
  // ignore: unused_field
  final $Res Function($Val) _then;

  /// Create a copy of BtClassicSettings
  /// with the given fields replaced by the non-null parameter values.
  @pragma('vm:prefer-inline')
  @override
  $Res call({
    Object? ackTriggerStrings = freezed,
    Object? ackString = freezed,
    Object? eotString = freezed,
    Object? dataLength = freezed,
    Object? transmissionDataTriggerString = freezed,
    Object? transmissionData = freezed,
  }) {
    return _then(_value.copyWith(
      ackTriggerStrings: freezed == ackTriggerStrings
          ? _value.ackTriggerStrings
          : ackTriggerStrings // ignore: cast_nullable_to_non_nullable
              as List<String>?,
      ackString: freezed == ackString
          ? _value.ackString
          : ackString // ignore: cast_nullable_to_non_nullable
              as String?,
      eotString: freezed == eotString
          ? _value.eotString
          : eotString // ignore: cast_nullable_to_non_nullable
              as String?,
      dataLength: freezed == dataLength
          ? _value.dataLength
          : dataLength // ignore: cast_nullable_to_non_nullable
              as int?,
      transmissionDataTriggerString: freezed == transmissionDataTriggerString
          ? _value.transmissionDataTriggerString
          : transmissionDataTriggerString // ignore: cast_nullable_to_non_nullable
              as String?,
      transmissionData: freezed == transmissionData
          ? _value.transmissionData
          : transmissionData // ignore: cast_nullable_to_non_nullable
              as String?,
    ) as $Val);
  }
}

/// @nodoc
abstract class _$$BtClassicSettingsImplCopyWith<$Res>
    implements $BtClassicSettingsCopyWith<$Res> {
  factory _$$BtClassicSettingsImplCopyWith(_$BtClassicSettingsImpl value,
          $Res Function(_$BtClassicSettingsImpl) then) =
      __$$BtClassicSettingsImplCopyWithImpl<$Res>;
  @override
  @useResult
  $Res call(
      {List<String>? ackTriggerStrings,
      String? ackString,
      String? eotString,
      int? dataLength,
      String? transmissionDataTriggerString,
      String? transmissionData});
}

/// @nodoc
class __$$BtClassicSettingsImplCopyWithImpl<$Res>
    extends _$BtClassicSettingsCopyWithImpl<$Res, _$BtClassicSettingsImpl>
    implements _$$BtClassicSettingsImplCopyWith<$Res> {
  __$$BtClassicSettingsImplCopyWithImpl(_$BtClassicSettingsImpl _value,
      $Res Function(_$BtClassicSettingsImpl) _then)
      : super(_value, _then);

  /// Create a copy of BtClassicSettings
  /// with the given fields replaced by the non-null parameter values.
  @pragma('vm:prefer-inline')
  @override
  $Res call({
    Object? ackTriggerStrings = freezed,
    Object? ackString = freezed,
    Object? eotString = freezed,
    Object? dataLength = freezed,
    Object? transmissionDataTriggerString = freezed,
    Object? transmissionData = freezed,
  }) {
    return _then(_$BtClassicSettingsImpl(
      ackTriggerStrings: freezed == ackTriggerStrings
          ? _value._ackTriggerStrings
          : ackTriggerStrings // ignore: cast_nullable_to_non_nullable
              as List<String>?,
      ackString: freezed == ackString
          ? _value.ackString
          : ackString // ignore: cast_nullable_to_non_nullable
              as String?,
      eotString: freezed == eotString
          ? _value.eotString
          : eotString // ignore: cast_nullable_to_non_nullable
              as String?,
      dataLength: freezed == dataLength
          ? _value.dataLength
          : dataLength // ignore: cast_nullable_to_non_nullable
              as int?,
      transmissionDataTriggerString: freezed == transmissionDataTriggerString
          ? _value.transmissionDataTriggerString
          : transmissionDataTriggerString // ignore: cast_nullable_to_non_nullable
              as String?,
      transmissionData: freezed == transmissionData
          ? _value.transmissionData
          : transmissionData // ignore: cast_nullable_to_non_nullable
              as String?,
    ));
  }
}

/// @nodoc

class _$BtClassicSettingsImpl implements _BtClassicSettings {
  const _$BtClassicSettingsImpl(
      {final List<String>? ackTriggerStrings,
      this.ackString,
      this.eotString,
      this.dataLength,
      this.transmissionDataTriggerString,
      this.transmissionData})
      : _ackTriggerStrings = ackTriggerStrings;

  final List<String>? _ackTriggerStrings;
  @override
  List<String>? get ackTriggerStrings {
    final value = _ackTriggerStrings;
    if (value == null) return null;
    if (_ackTriggerStrings is EqualUnmodifiableListView)
      return _ackTriggerStrings;
    // ignore: implicit_dynamic_type
    return EqualUnmodifiableListView(value);
  }

  @override
  final String? ackString;
  @override
  final String? eotString;
  @override
  final int? dataLength;
  @override
  final String? transmissionDataTriggerString;
  @override
  final String? transmissionData;

  @override
  String toString() {
    return 'BtClassicSettings(ackTriggerStrings: $ackTriggerStrings, ackString: $ackString, eotString: $eotString, dataLength: $dataLength, transmissionDataTriggerString: $transmissionDataTriggerString, transmissionData: $transmissionData)';
  }

  @override
  bool operator ==(Object other) {
    return identical(this, other) ||
        (other.runtimeType == runtimeType &&
            other is _$BtClassicSettingsImpl &&
            const DeepCollectionEquality()
                .equals(other._ackTriggerStrings, _ackTriggerStrings) &&
            (identical(other.ackString, ackString) ||
                other.ackString == ackString) &&
            (identical(other.eotString, eotString) ||
                other.eotString == eotString) &&
            (identical(other.dataLength, dataLength) ||
                other.dataLength == dataLength) &&
            (identical(other.transmissionDataTriggerString,
                    transmissionDataTriggerString) ||
                other.transmissionDataTriggerString ==
                    transmissionDataTriggerString) &&
            (identical(other.transmissionData, transmissionData) ||
                other.transmissionData == transmissionData));
  }

  @override
  int get hashCode => Object.hash(
      runtimeType,
      const DeepCollectionEquality().hash(_ackTriggerStrings),
      ackString,
      eotString,
      dataLength,
      transmissionDataTriggerString,
      transmissionData);

  /// Create a copy of BtClassicSettings
  /// with the given fields replaced by the non-null parameter values.
  @JsonKey(includeFromJson: false, includeToJson: false)
  @override
  @pragma('vm:prefer-inline')
  _$$BtClassicSettingsImplCopyWith<_$BtClassicSettingsImpl> get copyWith =>
      __$$BtClassicSettingsImplCopyWithImpl<_$BtClassicSettingsImpl>(
          this, _$identity);
}

abstract class _BtClassicSettings implements BtClassicSettings {
  const factory _BtClassicSettings(
      {final List<String>? ackTriggerStrings,
      final String? ackString,
      final String? eotString,
      final int? dataLength,
      final String? transmissionDataTriggerString,
      final String? transmissionData}) = _$BtClassicSettingsImpl;

  @override
  List<String>? get ackTriggerStrings;
  @override
  String? get ackString;
  @override
  String? get eotString;
  @override
  int? get dataLength;
  @override
  String? get transmissionDataTriggerString;
  @override
  String? get transmissionData;

  /// Create a copy of BtClassicSettings
  /// with the given fields replaced by the non-null parameter values.
  @override
  @JsonKey(includeFromJson: false, includeToJson: false)
  _$$BtClassicSettingsImplCopyWith<_$BtClassicSettingsImpl> get copyWith =>
      throw _privateConstructorUsedError;
}
