enum StopBits {
  stopBits_1(1),
  stopBits_1_5(3),
  stopBits_2(2);

  const StopBits(this.value);
  final int value;

  factory StopBits.fromValue(int value) {
    switch (value) {
      case 2:
        return StopBits.stopBits_2;
      case 3:
        return StopBits.stopBits_1_5;
      default:
        return StopBits.stopBits_1;
    }
  }
}
