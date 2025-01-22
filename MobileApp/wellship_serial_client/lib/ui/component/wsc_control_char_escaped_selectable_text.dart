import 'package:flutter/material.dart';

class WscControlCharEscapedSelectableText extends StatelessWidget {
  const WscControlCharEscapedSelectableText(this.data, {super.key});
  final String data;

  @override
  Widget build(BuildContext context) {
    final escapedData = data
        .replaceAllMapped(RegExp(r'[\x00-\x20]'), (x) => String.fromCharCode(0x2400 + x.group(0)!.codeUnitAt(0)))
        .replaceAll(RegExp(r'[\x7f]'), String.fromCharCode(0x2421));
    return SelectableText(escapedData);
  }
}
