import 'package:flutter/material.dart';

class WscSelectDialog<T> extends StatelessWidget {
  const WscSelectDialog({
    super.key,
    this.titleMapper,
    this.subtitleMapper,
    required this.title,
    required this.items,
    required this.groupValue,
  });

  final String Function(T)? titleMapper;
  final String Function(T)? subtitleMapper;

  final String title;
  final List<T> items;
  final T groupValue;

  @override
  Widget build(BuildContext context) {
    return AlertDialog(
      title: Text(title),
      content: SingleChildScrollView(
        child: Column(
          children: items
              .map(
                (x) => RadioListTile(
                  value: x,
                  title: titleMapper != null ? Text(titleMapper!(x)) : Text(x.toString()),
                  subtitle: subtitleMapper != null ? Text(subtitleMapper!(x)) : null,
                  groupValue: groupValue,
                  onChanged: (value) => Navigator.pop(context, value),
                ),
              )
              .toList(),
        ),
      ),
    );
  }
}
