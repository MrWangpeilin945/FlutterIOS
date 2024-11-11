import 'package:flutter/material.dart';

class WscMenuButton extends StatelessWidget {
  const WscMenuButton({super.key, required this.onPressed, required this.text});
  final void Function() onPressed;
  final String text;

  @override
  Widget build(BuildContext context) {
    return Container(
      margin: const EdgeInsets.all(20),
      child: OutlinedButton(
        onPressed: onPressed,
        style: ButtonStyle(
          fixedSize: WidgetStateProperty.all(const Size.fromHeight(140)),
          side: WidgetStateProperty.all(BorderSide(width: 8, color: Theme.of(context).colorScheme.primary)),
        ),
        child: Text(
          text,
          style: const TextStyle(fontSize: 80),
        ),
      ),
    );
  }
}
