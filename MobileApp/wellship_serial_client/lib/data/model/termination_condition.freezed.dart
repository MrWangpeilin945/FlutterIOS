// coverage:ignore-file
// GENERATED CODE - DO NOT MODIFY BY HAND
// ignore_for_file: type=lint
// ignore_for_file: unused_element, deprecated_member_use, deprecated_member_use_from_same_package, use_function_type_syntax_for_parameters, unnecessary_const, avoid_init_to_null, invalid_override_different_default_values_named, prefer_expression_function_bodies, annotate_overrides, invalid_annotation_target, unnecessary_question_mark

part of 'termination_condition.dart';

// **************************************************************************
// FreezedGenerator
// **************************************************************************

T _$identity<T>(T value) => value;

final _privateConstructorUsedError = UnsupportedError(
    'It seems like you constructed your class using `MyClass._()`. This constructor is only meant to be used by freezed and you are not supposed to need it nor use it.\nPlease check the documentation here for more information: https://github.com/rrousselGit/freezed#adding-getters-and-methods-to-our-models');

/// @nodoc
mixin _$TerminationCondition {
  List<String> get ackTriggers => throw _privateConstructorUsedError;
  String? get ackString => throw _privateConstructorUsedError;
  String? get eotString => throw _privateConstructorUsedError;
  int? get dataLength => throw _privateConstructorUsedError;

  /// Create a copy of TerminationCondition
  /// with the given fields replaced by the non-null parameter values.
  @JsonKey(includeFromJson: false, includeToJson: false)
  $TerminationConditionCopyWith<TerminationCondition> get copyWith =>
      throw _privateConstructorUsedError;
}

/// @nodoc
abstract class $TerminationConditionCopyWith<$Res> {
  factory $TerminationConditionCopyWith(TerminationCondition value,
          $Res Function(TerminationCondition) then) =
      _$TerminationConditionCopyWithImpl<$Res, TerminationCondition>;
  @useResult
  $Res call(
      {List<String> ackTriggers,
      String? ackString,
      String? eotString,
      int? dataLength});
}

/// @nodoc
class _$TerminationConditionCopyWithImpl<$Res,
        $Val extends TerminationCondition>
    implements $TerminationConditionCopyWith<$Res> {
  _$TerminationConditionCopyWithImpl(this._value, this._then);

  // ignore: unused_field
  final $Val _value;
  // ignore: unused_field
  final $Res Function($Val) _then;

  /// Create a copy of TerminationCondition
  /// with the given fields replaced by the non-null parameter values.
  @pragma('vm:prefer-inline')
  @override
  $Res call({
    Object? ackTriggers = null,
    Object? ackString = freezed,
    Object? eotString = freezed,
    Object? dataLength = freezed,
  }) {
    return _then(_value.copyWith(
      ackTriggers: null == ackTriggers
          ? _value.ackTriggers
          : ackTriggers // ignore: cast_nullable_to_non_nullable
              as List<String>,
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
    ) as $Val);
  }
}

/// @nodoc
abstract class _$$TerminationConditionImplCopyWith<$Res>
    implements $TerminationConditionCopyWith<$Res> {
  factory _$$TerminationConditionImplCopyWith(_$TerminationConditionImpl value,
          $Res Function(_$TerminationConditionImpl) then) =
      __$$TerminationConditionImplCopyWithImpl<$Res>;
  @override
  @useResult
  $Res call(
      {List<String> ackTriggers,
      String? ackString,
      String? eotString,
      int? dataLength});
}

/// @nodoc
class __$$TerminationConditionImplCopyWithImpl<$Res>
    extends _$TerminationConditionCopyWithImpl<$Res, _$TerminationConditionImpl>
    implements _$$TerminationConditionImplCopyWith<$Res> {
  __$$TerminationConditionImplCopyWithImpl(_$TerminationConditionImpl _value,
      $Res Function(_$TerminationConditionImpl) _then)
      : super(_value, _then);

  /// Create a copy of TerminationCondition
  /// with the given fields replaced by the non-null parameter values.
  @pragma('vm:prefer-inline')
  @override
  $Res call({
    Object? ackTriggers = null,
    Object? ackString = freezed,
    Object? eotString = freezed,
    Object? dataLength = freezed,
  }) {
    return _then(_$TerminationConditionImpl(
      ackTriggers: null == ackTriggers
          ? _value._ackTriggers
          : ackTriggers // ignore: cast_nullable_to_non_nullable
              as List<String>,
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
    ));
  }
}

/// @nodoc

class _$TerminationConditionImpl implements _TerminationCondition {
  const _$TerminationConditionImpl(
      {final List<String> ackTriggers = const [],
      this.ackString,
      this.eotString,
      this.dataLength})
      : _ackTriggers = ackTriggers;

  final List<String> _ackTriggers;
  @override
  @JsonKey()
  List<String> get ackTriggers {
    if (_ackTriggers is EqualUnmodifiableListView) return _ackTriggers;
    // ignore: implicit_dynamic_type
    return EqualUnmodifiableListView(_ackTriggers);
  }

  @override
  final String? ackString;
  @override
  final String? eotString;
  @override
  final int? dataLength;

  @override
  String toString() {
    return 'TerminationCondition(ackTriggers: $ackTriggers, ackString: $ackString, eotString: $eotString, dataLength: $dataLength)';
  }

  @override
  bool operator ==(Object other) {
    return identical(this, other) ||
        (other.runtimeType == runtimeType &&
            other is _$TerminationConditionImpl &&
            const DeepCollectionEquality()
                .equals(other._ackTriggers, _ackTriggers) &&
            (identical(other.ackString, ackString) ||
                other.ackString == ackString) &&
            (identical(other.eotString, eotString) ||
                other.eotString == eotString) &&
            (identical(other.dataLength, dataLength) ||
                other.dataLength == dataLength));
  }

  @override
  int get hashCode => Object.hash(
      runtimeType,
      const DeepCollectionEquality().hash(_ackTriggers),
      ackString,
      eotString,
      dataLength);

  /// Create a copy of TerminationCondition
  /// with the given fields replaced by the non-null parameter values.
  @JsonKey(includeFromJson: false, includeToJson: false)
  @override
  @pragma('vm:prefer-inline')
  _$$TerminationConditionImplCopyWith<_$TerminationConditionImpl>
      get copyWith =>
          __$$TerminationConditionImplCopyWithImpl<_$TerminationConditionImpl>(
              this, _$identity);
}

abstract class _TerminationCondition implements TerminationCondition {
  const factory _TerminationCondition(
      {final List<String> ackTriggers,
      final String? ackString,
      final String? eotString,
      final int? dataLength}) = _$TerminationConditionImpl;

  @override
  List<String> get ackTriggers;
  @override
  String? get ackString;
  @override
  String? get eotString;
  @override
  int? get dataLength;

  /// Create a copy of TerminationCondition
  /// with the given fields replaced by the non-null parameter values.
  @override
  @JsonKey(includeFromJson: false, includeToJson: false)
  _$$TerminationConditionImplCopyWith<_$TerminationConditionImpl>
      get copyWith => throw _privateConstructorUsedError;
}
