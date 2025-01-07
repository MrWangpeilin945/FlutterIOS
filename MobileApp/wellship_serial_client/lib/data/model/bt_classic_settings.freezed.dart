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
  String? get deviceName => throw _privateConstructorUsedError;
  String? get address => throw _privateConstructorUsedError;

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
  $Res call({String? deviceName, String? address});
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
    Object? deviceName = freezed,
    Object? address = freezed,
  }) {
    return _then(_value.copyWith(
      deviceName: freezed == deviceName
          ? _value.deviceName
          : deviceName // ignore: cast_nullable_to_non_nullable
              as String?,
      address: freezed == address
          ? _value.address
          : address // ignore: cast_nullable_to_non_nullable
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
  $Res call({String? deviceName, String? address});
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
    Object? deviceName = freezed,
    Object? address = freezed,
  }) {
    return _then(_$BtClassicSettingsImpl(
      deviceName: freezed == deviceName
          ? _value.deviceName
          : deviceName // ignore: cast_nullable_to_non_nullable
              as String?,
      address: freezed == address
          ? _value.address
          : address // ignore: cast_nullable_to_non_nullable
              as String?,
    ));
  }
}

/// @nodoc

class _$BtClassicSettingsImpl implements _BtClassicSettings {
  const _$BtClassicSettingsImpl({this.deviceName, this.address});

  @override
  final String? deviceName;
  @override
  final String? address;

  @override
  String toString() {
    return 'BtClassicSettings(deviceName: $deviceName, address: $address)';
  }

  @override
  bool operator ==(Object other) {
    return identical(this, other) ||
        (other.runtimeType == runtimeType &&
            other is _$BtClassicSettingsImpl &&
            (identical(other.deviceName, deviceName) ||
                other.deviceName == deviceName) &&
            (identical(other.address, address) || other.address == address));
  }

  @override
  int get hashCode => Object.hash(runtimeType, deviceName, address);

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
      {final String? deviceName,
      final String? address}) = _$BtClassicSettingsImpl;

  @override
  String? get deviceName;
  @override
  String? get address;

  /// Create a copy of BtClassicSettings
  /// with the given fields replaced by the non-null parameter values.
  @override
  @JsonKey(includeFromJson: false, includeToJson: false)
  _$$BtClassicSettingsImplCopyWith<_$BtClassicSettingsImpl> get copyWith =>
      throw _privateConstructorUsedError;
}
