enum TransMethod {
  none,
  wired,
  btClassic,
  ble;

  factory TransMethod.fromString(String value) {
    switch (value.toLowerCase()) {
      case 'wired':
        return TransMethod.wired;
      case 'spp':
        return TransMethod.btClassic;
      case 'ble':
        return TransMethod.ble;
      default:
        return TransMethod.none;
    }
  }
}
