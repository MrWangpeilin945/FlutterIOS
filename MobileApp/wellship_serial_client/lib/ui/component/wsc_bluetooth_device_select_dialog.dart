import 'package:flutter/material.dart';
import 'package:flutter_bluetooth_serial/flutter_bluetooth_serial.dart';
import 'package:hooks_riverpod/hooks_riverpod.dart';
import 'package:wellship_serial_client/data/provider/bt_classic_devices_provider.dart';

class WscBluetoothDeviceSelectDialog extends HookConsumerWidget {
  const WscBluetoothDeviceSelectDialog({
    super.key,
    required this.title,
    this.groupValue,
  });
  final String title;
  final BluetoothDevice? groupValue;

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final deviceStream = ref.watch(btClassicDevicesProvider);
    return AlertDialog(
      title: Text(title),
      content: SingleChildScrollView(
        child: Column(
          children: deviceStream.value
                  ?.map((x) => RadioListTile(
                        value: x.device,
                        title: Text(x.device.name ?? ''),
                        subtitle: Text(x.device.address),
                        groupValue: groupValue,
                        onChanged: (value) {
                          Navigator.pop(context, value);
                        },
                      ))
                  .toList() ??
              [],
        ),
      ),
    );
  }
}
