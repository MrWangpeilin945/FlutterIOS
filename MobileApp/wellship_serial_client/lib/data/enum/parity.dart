enum Parity {
  none(0),
  odd(1),
  even(2),
  mark(3),
  space(4);

  const Parity(this.value);
  final int value;

  factory Parity.fromValue(int value) {
    switch (value) {
      case 1:
        return Parity.odd;
      case 2:
        return Parity.even;
      case 3:
        return Parity.mark;
      case 4:
        return Parity.space;
      default:
        return Parity.none;
    }
  }
  factory Parity.fromString(String value) {
    switch (value.toLowerCase()) {
      case 'odd':
        return Parity.odd;
      case 'even':
        return Parity.even;
      case 'mark':
        return Parity.mark;
      case 'space':
        return Parity.space;
      default:
        return Parity.none;
    }
  }
}
