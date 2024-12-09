import 'package:auto_route/auto_route.dart';
import 'package:flutter/material.dart';
import 'package:flutter_hooks/flutter_hooks.dart';
import 'package:hooks_riverpod/hooks_riverpod.dart';
import 'package:wellship_serial_client/data/enum/parity.dart';
import 'package:wellship_serial_client/data/enum/stop_bits.dart';
import 'package:wellship_serial_client/data/model/termination_condition.dart';
import 'package:wellship_serial_client/data/model/wired_settings.dart';
import 'package:wellship_serial_client/data/provider/termination_condition_provider.dart';
import 'package:wellship_serial_client/ui/component/wsc_input_dialog.dart';
import 'package:wellship_serial_client/ui/component/wsc_select_dialog.dart';
import 'package:wellship_serial_client/ui/route/app_route.dart';
import 'package:wellship_serial_client/ui/route/app_route.gr.dart';

@RoutePage()
class WiredSerialSettingsPage extends HookConsumerWidget {
  const WiredSerialSettingsPage({super.key});

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final router = ref.read(routerProvider);
    final wiredSettings = ref.read(wiredSettingsProvider);
    final baudState = useState<int>(wiredSettings.baud);
    final dataBitsState = useState<int>(wiredSettings.dataBits);
    final parityBitState = useState<Parity>(wiredSettings.parity);
    final stopBitState = useState<StopBits>(wiredSettings.stopBits);
    final useDtrState = useState<bool>(wiredSettings.useDtr);
    final useRtsState = useState<bool>(wiredSettings.useRts);
    final ackString = useState<String>('\x06');
    final ackTriggers = useState<List<String>>([]);
    final eotString = useState<String>('\r\n');
    final dataLength = useState<int>(128);

    return Scaffold(
      appBar: AppBar(
        title: const Text('有線接続'),
        backgroundColor: Theme.of(context).colorScheme.inversePrimary,
      ),
      body: SingleChildScrollView(
        child: Center(
          child: Padding(
            padding: const EdgeInsets.all(16.0),
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.stretch,
              mainAxisAlignment: MainAxisAlignment.start,
              children: [
                ListTile(
                    leading: const Icon(Icons.cable),
                    title: const Text("接続設定"),
                    textColor: Theme.of(context).colorScheme.primary),
                ListTile(
                  title: const Text("ボーレート"),
                  subtitle: Text(baudState.value.toString()),
                  onTap: () async {
                    final controller =
                        TextEditingController.fromValue(TextEditingValue(text: baudState.value.toString()));
                    await showDialog(
                      context: context,
                      builder: (context) {
                        return WscInputDialog(
                          title: "ボーレートを入力してください",
                          keyboardType: TextInputType.number,
                          controller: controller,
                          onConfirmed: () {
                            baudState.value = int.tryParse(controller.text) ?? 0;
                            Navigator.pop(context);
                          },
                        );
                      },
                    );
                  },
                ),
                ListTile(
                  title: const Text("データビット数"),
                  subtitle: Text(dataBitsState.value.toString()),
                  onTap: () async {
                    await showDialog(
                      context: context,
                      builder: (context) {
                        return WscSelectDialog(
                          title: "データビット数を入力してください",
                          items: const [5, 6, 7, 8],
                          mapper: (x) => RadioListTile(
                            value: x,
                            title: Text(x.toString()),
                            groupValue: dataBitsState.value,
                            onChanged: (value) {
                              if (value != null) {
                                dataBitsState.value = value;
                              }
                              Navigator.pop(context);
                            },
                          ),
                        );
                      },
                    );
                  },
                ),
                ListTile(
                  title: const Text("パリティビット"),
                  subtitle: Text(parityBitState.value.toString()),
                  onTap: () async {
                    await showDialog(
                      context: context,
                      builder: (context) {
                        return WscSelectDialog(
                          title: "パリティビットを選択してください",
                          items: Parity.values,
                          mapper: (x) => RadioListTile(
                              value: x,
                              title: Text(x.name),
                              groupValue: parityBitState.value,
                              onChanged: (value) {
                                parityBitState.value = value ?? Parity.none;
                                Navigator.pop(context);
                              }),
                        );
                      },
                    );
                  },
                ),
                ListTile(
                  title: const Text("ストップビット"),
                  subtitle: Text(stopBitState.value.toString()),
                  onTap: () async {
                    await showDialog(
                      context: context,
                      builder: (context) {
                        return WscSelectDialog(
                          title: "ストップビットを選択してください",
                          items: StopBits.values,
                          mapper: (x) => RadioListTile(
                            value: x,
                            groupValue: stopBitState.value,
                            title: Text(x.name),
                            onChanged: (value) {
                              stopBitState.value = value ?? StopBits.stopBits_1;
                              Navigator.pop(context);
                            },
                          ),
                        );
                      },
                    );
                  },
                ),
                SwitchListTile(
                  title: const Text("DTRを使用する"),
                  value: useDtrState.value,
                  onChanged: (x) => useDtrState.value = x,
                ),
                SwitchListTile(
                  title: const Text("RTSを使用する"),
                  value: useRtsState.value,
                  onChanged: (x) => useRtsState.value = x,
                ),
                ListTile(
                    leading: const Icon(Icons.swap_horiz),
                    title: const Text("通信設定"),
                    textColor: Theme.of(context).colorScheme.primary),
                ListTile(
                  title: const Text("Ack文字列"),
                  subtitle: Text(String.fromCharCode(0x2406)),
                  onTap: () {},
                ),
                ListTile(
                  title: const Text("Ack送信条件文字列"),
                  subtitle: Text("${String.fromCharCode(0x2403)} / ${String.fromCharCode(0x240d)}"),
                  onTap: () {},
                ),
                ListTile(
                  title: const Text("終了条件文字列"),
                  subtitle: Text(String.fromCharCode(0x2404)),
                  onTap: () {},
                ),
                ListTile(
                  title: const Text("終了条件バイト数"),
                  subtitle: const Text("128"),
                  onTap: () {},
                ),
              ],
            ),
          ),
        ),
      ),
      bottomNavigationBar: Padding(
        padding: const EdgeInsets.fromLTRB(16, 0, 16, 32),
        child: FilledButton(
            onPressed: () {
              ref.read(wiredSettingsProvider.notifier).state = WiredSettings(
                  baud: baudState.value,
                  dataBits: dataBitsState.value,
                  parity: parityBitState.value,
                  stopBits: stopBitState.value,
                  useRts: useRtsState.value,
                  useDtr: useDtrState.value);
              ref.read(terminationConditionProvider.notifier).state = TerminationCondition(
                ackString: ackString.value,
                ackTriggers: ackTriggers.value,
                eotString: eotString.value,
                dataLength: dataLength.value,
              );
              router.push(const WiredSerialCommunicationRoute());
            },
            child: const Text('読み取り開始')),
      ),
    );
  }
}
