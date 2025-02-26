import 'dart:convert';
import 'package:flutter/material.dart';
import 'package:http/http.dart' as http;
import 'config.dart';
import 'pre-task-plan-form.dart';

class PreTaskPlanReviewPage extends StatefulWidget {
  final int inputId;
  final Map<String, dynamic> ptpDocument;
  
  const PreTaskPlanReviewPage({Key? key,
    required this.inputId,
    required this.ptpDocument}) : super(key: key);
  
  @override
  _PreTaskPlanReviewPageState createState() => _PreTaskPlanReviewPageState();
}

class _PreTaskPlanReviewPageState extends State<PreTaskPlanReviewPage> {
  final _formKey = GlobalKey<FormState>();
  late TextEditingController titleController;
  late TextEditingController summaryController;
  List<TextEditingController> sectionHeaderControllers = [];
  List<TextEditingController> sectionContentControllers = [];
  bool _isLoading = false;

  @override
  void initState() {
    super.initState();
    // populate controllers with JSON document values
    titleController =
        TextEditingController(text: widget.ptpDocument['title'] ?? '');

    if (widget.ptpDocument['sections'] != null &&
        widget.ptpDocument['sections'] is List) {
      for (var section in widget.ptpDocument['sections']) {
        sectionHeaderControllers.add(
            TextEditingController(text: section['header'] ?? ''));
        sectionContentControllers.add(
            TextEditingController(text: parseContent(section['content'])));
      }
    }

    summaryController = TextEditingController(text: parseContent(widget.ptpDocument['summary']));
  }
  // Disgustingly nested.
  String parseContent(dynamic content) {
    if (content is String) {
      return content;
    } else if (content is List) {
      // Join if strings, if objects encode
      // assuming they can be cast to strings
      return content.map((item) {
        // if item is a Map return the encodedItem
        if (item is Map) {
          return jsonEncode(item);
        } else if (item is List) {
          // recursive parse
          return parseContent(item);
      } else {
          return item.toString();
        }
      }).join("\n");
    } else {
      // fallback
      return "no String No List";
    }
  }

  @override
  void dispose() {
    titleController.dispose();
    summaryController.dispose();
    for (var ctrl in sectionHeaderControllers) ctrl.dispose();
    for (var ctrl in sectionContentControllers) ctrl.dispose();
    super.dispose();
  }
  
  Future<void> _finalizeDocument() async {
    if (!_formKey.currentState!.validate()) return;

    setState(() {
      _isLoading = true;
    });

    Map<String, dynamic> signSubmitData = {
      'signedBy': "USERIDOFCREATOR",
      'status': 'safetyReview',
      'updatedGeneratedJson': jsonEncode({
        'title': titleController.text,
        'sections': List.generate(sectionHeaderControllers.length, (index) {
          return {
            'header': sectionHeaderControllers[index].text,
            'content': sectionContentControllers[index].text,
          };
        }),
        'summary': summaryController.text,
      }),
    };

    var url = Uri.parse("${ApiConfig.signAndSubmitEndpoint}/${widget.inputId}");

    try {
      final response = await http.post(
        url,
        headers: {'Content-Type': 'application/json'},
        body: jsonEncode(signSubmitData),
      );
      if (response.statusCode == 201 || response.statusCode == 200) {
        ScaffoldMessenger.of(context).showSnackBar(
          SnackBar(content: Text('Form signed and submitted successfully!')),
        );
        Navigator.pushReplacement(
          context,
          MaterialPageRoute(builder: (context) => PreTaskPlanForm()),
        );
      } else {
        ScaffoldMessenger.of(context).showSnackBar(
          SnackBar(content: Text(
              'Failed to sign and submit: ${response.body} and ${response
                  .statusCode}')),
        );
      }
    } catch (error) {
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(content: Text('An error occurred: $error')),
      );
    } finally {
      setState(() {
        _isLoading = false;
      });
    }
  }
  
  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(title: const Text('Review & Edit PTP')),
      body: Form(
        key: _formKey,
        child: ListView(
          padding: const EdgeInsets.all(16.0),
          children: [
            TextFormField(
              controller: titleController,
              decoration: const InputDecoration(labelText: 'Title',
              labelStyle: TextStyle(
                fontSize: 20,
                fontWeight: FontWeight.bold,
              ),
              ),
              validator: (value) => value == null || value.isEmpty ? 'Please enter a title' : null,
            ),
            const SizedBox(height: 16),
            ...List.generate(sectionHeaderControllers.length, (index) {
              return Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  Text('${sectionHeaderControllers[index].text.toUpperCase()}',
                  style: TextStyle(fontWeight: FontWeight.bold, fontSize:16),
                  ),
                  const SizedBox(height: 8),
                  Text('Please review content'),
                  TextFormField(
                    controller: sectionContentControllers[index],
                    decoration: const InputDecoration(
                      labelText: 'Section content',
                      border: OutlineInputBorder(),
                    ),
                    maxLines: null,
                    keyboardType: TextInputType.multiline,
                    validator: (value) => value == null || value.isEmpty ? 'Please enter content' : null,
                  ),
                  const SizedBox(height: 16),
                ],
              );
            }),
            TextFormField(
              controller: summaryController,
              decoration: const InputDecoration(labelText: 'Summary'),
              validator: (value) => value == null || value.isEmpty ? 'Please enter a summary' : null,
            ),
            const SizedBox(height: 24),
            ElevatedButton(
              onPressed: _isLoading
                  ? null
                  : () {
                showDialog(
                  context: context,
                  builder: (BuildContext context) {
                    return AlertDialog(
                      title: const Text('Confirm Submission'),
                      content: const Text('Are you sure you want to sign and submit the form?'),
                      actions: [
                        TextButton(
                          child: const Text('Cancel'),
                          onPressed: () {
                            Navigator.of(context).pop();
                          },
                        ),
                        ElevatedButton(
                          child: _isLoading
                              ? SizedBox(
                            width: 20,
                            height: 20,
                            child: CircularProgressIndicator(
                              valueColor: AlwaysStoppedAnimation<Color>(Colors.white),
                              strokeWidth: 2.0,
                            ),
                          )
                              : const Text('Confirm'),
                          onPressed: () async {
                            // Close the dialog
                            Navigator.of(context).pop();
                            // Start loading state
                            setState(() {
                              _isLoading = true;
                            });
                            // Call your async function
                            await _finalizeDocument();
                            // When complete, reset loading state
                            if (mounted) {
                              setState(() {
                                _isLoading = false;
                              });
                            }
                          },
                        ),
                      ],
                    );
                  },
                );
              },
              child: _isLoading
                  ? SizedBox(
                width: 20,
                height: 20,
                child: CircularProgressIndicator(
                  valueColor: AlwaysStoppedAnimation<Color>(Colors.white),
                  strokeWidth: 2.0,
                ),
              )
                  : const Text('Sign & Submit Form'),
            ),
          ],
        ),
      ),
    );
  }
}