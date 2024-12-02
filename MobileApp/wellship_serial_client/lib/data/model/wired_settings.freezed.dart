// coverage:ignore-file
// GENERATED CODE - DO NOT MODIFY BY HAND
// ignore_for_file: type=lint
// ignore_for_file: unused_element, deprecated_member_use, deprecated_member_use_from_same_package, use_function_type_syntax_for_parameters, unnecessary_const, avoid_init_to_null, invalid_override_different_default_values_named, prefer_expression_function_bodies, annotate_overrides, invalid_annotation_target, unnecessary_question_mark

part of 'wired_settings.dart';

// **************************************************************************
// FreezedGenerator
// **************************************************************************

T _$identity<T>(T value) => value;

final _privateConstructorUsedError = UnsupportedError(
    'It seems like you constructed your class using `MyClass._()`. This constructor is only meant to be used by freezed and you are not supposed to need it nor use it.\nPlease check the documentation here for more information: https://github.com/rrousselGit/freezed#adding-getters-and-methods-to-our-models');

/// @nodoc
mixin _$WiredSettings {
  int get baud => throw _privateConstructorUsedError;
  int get dataBits => throw _privateConstructorUsedError;
  Parity get parity => throw _privateConstructorUsedError;
  StopBits get stopBits => throw _privateConstructorUsedError;
  bool get useRts => throw _privateConstructorUsedError;
  bool get useDtr => throw _privateConstructorUsedError;

  /// Create a copy of WiredSettings
  /// with the given fields replaced by the non-null parameter values.
  @JsonKey(includeFromJson: false, includeToJson: false)
  $WiredSettingsCopyWith<WiredSettings> get copyWith =>
      throw _privateConstructorUsedError;
}

/// @nodoc
abstract class $WiredSettingsCopyWith<$Res> {
  factory $WiredSettingsCopyWith(
          WiredSettings value, $Res Function(WiredSettings) then) =
      _$WiredSettingsCopyWithImpl<$Res, WiredSettings>;
  @useResult
  $Res call(
      {int baud,
      int dataBits,
      Parity parity,
      StopBits stopBits,
      bool useRts,
      bool useDtr});
}

/// @nodoc
class _$WiredSettingsCopyWithImpl<$Res, $Val extends WiredSettings>
    implements $WiredSettingsCopyWith<$Res> {
  _$WiredSettingsCopyWithImpl(this._value, this._then);

  // ignore: unused_field
  final $Val _value;
  // ignore: unused_field
  final $Res Function($Val) _then;

  /// Create a copy of WiredSettings
  /// with the given fields replaced by the non-null parameter values.
  @pragma('vm:prefer-inline')
  @override
  $Res call({
    Object? baud = null,
    Object? dataBits = null,
    Object? parity = null,
    Object? stopBits = null,
    Object? useRts = null,
    Object? useDtr = null,
  }) {
    return _then(_value.copyWith(
      baud: null == baud
          ? _value.baud
          : baud // ignore: cast_nullable_to_non_nullable
              as int,
      dataBits: null == dataBits
          ? _value.dataBits
          : dataBits // ignore: cast_nullable_to_non_nullable
              as int,
      parity: null == parity
          ? _value.parity
          : parity // ignore: cast_nullable_to_non_nullable
              as Parity,
      stopBits: null == stopBits
          ? _value.stopBits
          : stopBits // ignore: cast_nullable_to_non_nullable
              as StopBits,
      useRts: null == useRts
          ? _value.useRts
          : useRts // ignore: cast_nullable_to_non_nullable
              as bool,
      useDtr: null == useDtr
          ? _value.useDtr
          : useDtr // ignore: cast_nullable_to_non_nullable
              as bool,
    ) as $Val);
  }
}

/// @nodoc
abstract class _$$WiredSettingsImplCopyWith<$Res>
    implements $WiredSettingsCopyWith<$Res> {
  factory _$$WiredSettingsImplCopyWith(
          _$WiredSettingsImpl value, $Res Function(_$WiredSettingsImpl) then) =
      __$$WiredSettingsImplCopyWithImpl<$Res>;
  @override
  @useResult
  $Res call(
      {int baud,
      int dataBits,
      Parity parity,
      StopBits stopBits,
      bool useRts,
      bool useDtr});
}

/// @nodoc
class __$$WiredSettingsImplCopyWithImpl<$Res>
    extends _$WiredSettingsCopyWithImpl<$Res, _$WiredSettingsImpl>
    implements _$$WiredSettingsImplCopyWith<$Res> {
  __$$WiredSettingsImplCopyWithImpl(
      _$WiredSettingsImpl _value, $Res Function(_$WiredSettingsImpl) _then)
      : super(_value, _then);

  /// Create a copy of WiredSettings
  /// with the given fields replaced by the non-null parameter values.
  @pragma('vm:prefer-inline')
  @override
  $Res call({
    Object? baud = null,
    Object? dataBits = null,
    Object? parity = null,
    Object? stopBits = null,
    Object? useRts = null,
    Object? useDtr = null,
  }) {
    return _then(_$WiredSettingsImpl(
      baud: null == baud
          ? _value.baud
          : baud // ignore: cast_nullable_to_non_nullable
              as int,
      dataBits: null == dataBits
          ? _value.dataBits
          : dataBits // ignore: cast_nullable_to_non_nullable
              as int,
      parity: null == parity
          ? _value.parity
          : parity // ignore: cast_nullable_to_non_nullable
              as Parity,
      stopBits: null == stopBits
          ? _value.stopBits
          : stopBits // ignore: cast_nullable_to_non_nullable
              as StopBits,
      useRts: null == useRts
          ? _value.useRts
          : useRts // ignore: cast_nullable_to_non_nullable
              as bool,
      useDtr: null == useDtr
          ? _value.useDtr
          : useDtr // ignore: cast_nullable_to_non_nullable
              as bool,
    ));
  }
}

/// @nodoc

class _$WiredSettingsImpl implements _WiredSettings {
  const _$WiredSettingsImpl(
      {required this.baud,
      required this.dataBits,
      required this.parity,
      required this.stopBits,
      required this.useRts,
      required this.useDtr});

  @override
  final int baud;
  @override
  final int dataBits;
  @override
  final Parity parity;
  @override
  final StopBits stopBits;
  @override
  final bool useRts;
  @override
  final bool useDtr;

  @override
  String toString() {
    return 'WiredSettings(baud: $baud, dataBits: $dataBits, parity: $parity, stopBits: $stopBits, useRts: $useRts, useDtr: $useDtr)';
  }

  @override
  bool operator ==(Object other) {
    return identical(this, other) ||
        (other.runtimeType == runtimeType &&
            other is _$WiredSettingsImpl &&
            (identical(other.baud, baud) || other.baud == baud) &&
            (identical(other.dataBits, dataBits) ||
                other.dataBits == dataBits) &&
            (identical(other.parity, parity) || other.parity == parity) &&
            (identical(other.stopBits, stopBits) ||
                other.stopBits == stopBits) &&
            (identical(other.useRts, useRts) || other.useRts == useRts) &&
            (identical(other.useDtr, useDtr) || other.useDtr == useDtr));
  }

  @override
  int get hashCode => Object.hash(
      runtimeType, baud, dataBits, parity, stopBits, useRts, useDtr);

  /// Create a copy of WiredSettings
  /// with the given fields replaced by the non-null parameter values.
  @JsonKey(includeFromJson: false, includeToJson: false)
  @override
  @pragma('vm:prefer-inline')
  _$$WiredSettingsImplCopyWith<_$WiredSettingsImpl> get copyWith =>
      __$$WiredSettingsImplCopyWithImpl<_$WiredSettingsImpl>(this, _$identity);
}

abstract class _WiredSettings implements WiredSettings {
  const factory _WiredSettings(
      {required final int baud,
      required final int dataBits,
      required final Parity parity,
      required final StopBits stopBits,
      required final bool useRts,
      required final bool useDtr}) = _$WiredSettingsImpl;

  @override
  int get baud;
  @override
  int get dataBits;
  @override
  Parity get parity;
  @override
  StopBits get stopBits;
  @override
  bool get useRts;
  @override
  bool get useDtr;

  /// Create a copy of WiredSettings
  /// with the given fields replaced by the non-null parameter values.
  @override
  @JsonKey(includeFromJson: false, includeToJson: false)
  _$$WiredSettingsImplCopyWith<_$WiredSettingsImpl> get copyWith =>
      throw _privateConstructorUsedError;
}
