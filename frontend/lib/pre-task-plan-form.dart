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

  Future<void> _submitData() async {
    if (_formKey.currentState!.validate()) {
      _formKey.currentState!.save();
      
      Map<String, dynamic> data = {
        'workArea': workArea,
        'activityDescription': workActivity,
      };
      
      var url = Uri.parse(ApiConfig.preTaskPlanEndpoint);
      
      final response = await http.post(
        url,
        headers: {'Content-Type': 'application/json'},
        body: jsonEncode(data),
      );

      if (response.statusCode == 201) {
        // Success and parse JSON
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
            )
          )
        );
      } else {
        // Handle error
        ScaffoldMessenger.of(context).showSnackBar(
          SnackBar(content: Text('Submission failed: ${response.body}')),
        );
      }
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
                onPressed: _submitData,
                child: Text('Submit'),
              ),
            ],
          ),
        ),
      ),
    );
  }
}