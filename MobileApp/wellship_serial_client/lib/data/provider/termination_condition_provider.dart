import 'package:hooks_riverpod/hooks_riverpod.dart';
import 'package:wellship_serial_client/data/model/termination_condition.dart';

final terminationConditionProvider = StateProvider<TerminationCondition>((ref) => const TerminationCondition());
