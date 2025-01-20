import 'package:freezed_annotation/freezed_annotation.dart';
import 'package:shared_preferences/shared_preferences.dart';

part 'bt_classic_settings.freezed.dart';

@freezed
class BtClassicSettings with _$BtClassicSettings {
  const factory BtClassicSettings({
    String? deviceName,
    String? address,
  }) = _BtClassicSettings;

  static Future<BtClassicSettings> loadSettings() async {
    final sp = SharedPreferencesAsync();
    final deviceNameFuture = sp.getString('btcDeviceName');
    final deviceAddressFuture = sp.getString('btcDeviceAddress');
    return BtClassicSettings(
      deviceName: await deviceNameFuture,
      address: await deviceAddressFuture,
    );
  }

  static Future<void> saveSettings(BtClassicSettings settings) async {
    final sp = SharedPreferencesAsync();
    final deviceName = settings.deviceName;
    final address = settings.address;
    if (deviceName != null && address != null) {
      await sp.setString('btcDeviceName', deviceName);
      await sp.setString('btcDeviceAddress', address);
    }
  }
}
