import 'package:flutter/material.dart';

class WscSelectDialog<T extends Enum> extends StatelessWidget {
  const WscSelectDialog({
    super.key,
    required this.title,
    required this.items,
    required this.selected,
    required this.onSelect,
  });
  final String title;
  final ValueChanged<T?> onSelect;
  final T selected;
  final List<T> items;

  void defaultOnCanceled(BuildContext context) => Navigator.pop(context);

  @override
  Widget build(BuildContext context) {
    return AlertDialog(
      title: Text(title),
      content: SingleChildScrollView(
        child: Column(
          children: items
              .map(
                (x) => RadioListTile(
                  title: Text(x.name),
                  value: x.index,
                  groupValue: selected.index,
                  onChanged: (index) {
                    onSelect(index == null ? null : items[index]);
                  },
                ),
              )
              .toList(),
        ),
      ),
    );
  }
}
