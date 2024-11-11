import 'package:flutter/material.dart';

class WscMenuButton extends StatelessWidget {
  const WscMenuButton({super.key, required this.onPressed, required this.text});
  final void Function()? onPressed;
  final String text;

  @override
  Widget build(BuildContext context) {
    return Container(
      margin: const EdgeInsets.all(20),
      child: OutlinedButton(
        onPressed: onPressed,
        style: ButtonStyle(
            side: WidgetStateProperty.resolveWith((Set<WidgetState> states) {
              final disabled = states.contains(WidgetState.disabled);
              return BorderSide(
                width: 6,
                color: disabled ? Theme.of(context).disabledColor : Theme.of(context).colorScheme.primary,
              );
            }),
            minimumSize: WidgetStateProperty.all(const Size.fromHeight(100)),
            padding: WidgetStateProperty.all(const EdgeInsets.only(left: 50, right: 50))),
        child: Text(
          text,
          style: const TextStyle(fontSize: 40),
        ),
      ),
    );
  }
}
