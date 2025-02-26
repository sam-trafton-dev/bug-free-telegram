import 'package:flutter/material.dart';
import 'package:http/http.dart' as http;
import 'dart:convert';
import 'config.dart';
import 'pre-task-plan-review-page.dart';

class PreTaskPlanForm extends StatefulWidget {
  @override
  _PreTaskPlanFormState createState() => _PreTaskPlanFormState();
}

class _PreTaskPlanFormState extends State<PreTaskPlanForm> {
  final _formKey = GlobalKey<FormState>();
  String? workArea;
  String? workActivity;
  bool _isLoading = false;

  Future<void> _submitData() async {
    if (!_formKey.currentState!.validate()) return;
    _formKey.currentState!.save();

    setState(() {
      _isLoading = true;
    });

    Map<String, dynamic> data = {
      'workArea': workArea,
      'activityDescription': workActivity,
    };

    var url = Uri.parse(ApiConfig.preTaskPlanEndpoint);

    try {
      final response = await http.post(
        url,
        headers: {'Content-Type': 'application/json'},
        body: jsonEncode(data),
      );

      if (response.statusCode == 201) {
        var responseJson = jsonDecode(response.body);
        var inputId = responseJson['inputId'];
        var ptpDocument = responseJson['ptpDocument'];

        ScaffoldMessenger.of(context).showSnackBar(
          SnackBar(content: Text('Pre-Task Plan submitted successfully!')),
        );
        Navigator.push(
          context,
          MaterialPageRoute(
            builder: (context) => PreTaskPlanReviewPage(
              inputId: inputId,
              ptpDocument: ptpDocument,
            ),
          ),
        );
      } else {
        ScaffoldMessenger.of(context).showSnackBar(
          SnackBar(content: Text('Submission failed: ${response.body}')),
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
      appBar: AppBar(title: Text('Pre-Task Plan')),
      body: Padding(
        padding: const EdgeInsets.all(16.0),
        child: Form(
          key: _formKey,
          child: Column(
            children: [
              TextFormField(
                decoration: InputDecoration(labelText: 'Work Area'),
                validator: (value) {
                  if (value == null || value.isEmpty) {
                    return 'Please enter a work area';
                  }
                  return null;
                },
                onSaved: (value) => workArea = value,
              ),
              TextFormField(
                decoration: InputDecoration(labelText: 'Work Activity'),
                validator: (value) {
                  if (value == null || value.isEmpty) {
                    return 'Please enter work activity details';
                  }
                  return null;
                },
                onSaved: (value) => workActivity = value,
              ),
              SizedBox(height: 20),
              ElevatedButton(
                onPressed: _isLoading ? null : _submitData,
                child: _isLoading ?
                  SizedBox(
                    width: 20,
                    height: 20,
                    child: CircularProgressIndicator(
                      valueColor: AlwaysStoppedAnimation<Color>(Colors.white),
                      strokeWidth:2.0,
                    ),
                  ) : Text('Submit'),
              ),
            ],
          ),
        ),
      ),
    );
  }
}