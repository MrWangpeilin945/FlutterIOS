import 'package:flutter/material.dart';

class WscInputDialog extends StatelessWidget {
  const WscInputDialog({
    super.key,
    required this.title,
    required this.controller,
    required this.onConfirmed,
    this.onCanceled,
    this.keyboardType,
  });
  final String title;
  final TextEditingController controller;
  final VoidCallback onConfirmed;
  final VoidCallback? onCanceled;
  final TextInputType? keyboardType;

  void defaultOnCanceled(BuildContext context) => Navigator.pop(context);

  @override
  Widget build(BuildContext context) {
    return AlertDialog(
      title: Text(title),
      content: TextField(
        autofocus: true,
        keyboardType: keyboardType,
        controller: controller,
      ),
      actions: [
        TextButton(onPressed: onCanceled ?? () => defaultOnCanceled(context), child: const Text('キャンセル')),
        TextButton(onPressed: onConfirmed, child: const Text('OK')),
      ],
    );
  }
}
