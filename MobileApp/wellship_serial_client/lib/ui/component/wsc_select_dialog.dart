import 'package:flutter/material.dart';

class WscSelectDialog<T> extends StatelessWidget {
  const WscSelectDialog({
    super.key,
    required this.title,
    required this.items,
    required this.mapper,
  });
  final String title;
  final RadioListTile<T> Function(T) mapper;
  final List<T> items;

  @override
  Widget build(BuildContext context) {
    return AlertDialog(
      title: Text(title),
      content: SingleChildScrollView(
        child: Column(
          children: items.map(mapper).toList(),
        ),
      ),
    );
  }
}
