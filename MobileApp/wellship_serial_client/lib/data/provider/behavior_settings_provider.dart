import 'package:hooks_riverpod/hooks_riverpod.dart';
import 'package:wellship_serial_client/data/model/behavior_settings.dart';

final behaviorSettingsProvider = StateProvider<BehaviorSettings>((ref) => const BehaviorSettings());
