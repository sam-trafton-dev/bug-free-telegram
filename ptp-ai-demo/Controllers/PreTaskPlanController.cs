using Microsoft.AspNetCore.Mvc;
using ptp_ai_demo.Data;
using ptp_ai_demo.Models;
using Newtonsoft.Json;
using System.Text;

namespace ptp_ai_demo.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PreTaskPlanController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IConfiguration _configuration;
    private readonly HttpClient _httpClient;

    public PreTaskPlanController(ApplicationDbContext context, IConfiguration configuration, HttpClient httpClient)
    {
        _context = context;
        _configuration = configuration;
        _httpClient = httpClient;
    }

    [HttpPost]
    public async Task<IActionResult> CreatePreTaskPlan([FromBody] PreTaskPlanInput input)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        _context.PreTaskPlanInputs.Add(input);
        await _context.SaveChangesAsync();
        
        // Prompt for Agent, should also consider grounding Agent with specific documents
        string prompt = 
            "Generate a concise Pre-Task-Plan (PTP) document in plain JSON format that assesses hazards, risk level, and mitigations referencing OSHA standards for the following work scenario:\n" +
            $"Work Area: {input.WorkArea}\n" +
            $"Activity Description: {input.ActivityDescription}\n\n" +
            "Return the output as a single JSON object with exactly these keys:\n" +
            "  - \"title\": a plain string (no markdown or formatting),\n" +
            "  - \"sections\": an array of objects, each with exactly two keys: \"header\" (a plain string) and \"content\" (which is always a list of strings even if only a single string),\n" +
            "  - \"summary\": a plain string with no markdown formatting or nested JSON. Do not include any code fences, backticks, or additional text. " +
            "You must include the \"header\"s titled \"work steps\", \"hazards\", \"risk level\" , and \"mitigations\" " +
            "If you approach the token limit, ensure that you gracefully close all open brackets and braces so that the returned JSON is complete.";

        string ptpJson;

        try
        {
            ptpJson = await CallAzureOpenAiAsync(prompt);
        }
        catch (HttpRequestException ex)
        {
            return StatusCode(500, $"Error calling Azure OpenAI: {ex.Message}");
        }

        dynamic rawResult = JsonConvert.DeserializeObject(ptpJson) ?? throw new InvalidOperationException();
        string generatedContentForSql = rawResult.choices[0].message.content;
        System.Diagnostics.Debug.WriteLine("SQL JSON STRING: " + generatedContentForSql);
        var structuredPtp = JsonConvert.DeserializeObject<PtpDocument>(generatedContentForSql);
        
        
        if (rawResult == null || rawResult.choices == null || rawResult.choices.Count == 0)
        {
            // Provide a fallback JSON object
            System.Diagnostics.Debug.WriteLine("GENERATION FAILED");
            structuredPtp = new PtpDocument()
            {
                Title = "Generation Failed",
                Sections = new List<Section>(),
                Summary = "Boohoo"
            };
        }
        else
        {
            // Assume the generated text is JSON-formatted; if not, wrap it into an object.
            string generatedContent = rawResult.choices[0].message.content;
            System.Diagnostics.Debug.WriteLine($"Message content: {generatedContent}");
            // If your generatedContent is a JSON string, you might try deserializing it:
            try
            {
                structuredPtp = JsonConvert.DeserializeObject<PtpDocument>(generatedContent);
            }
            catch
            {
                System.Diagnostics.Debug.WriteLine("Wrapping plain text into an object");
                // If it's just plain text, wrap it in an object:
                structuredPtp = new PtpDocument()
                {
                    Title = "Generated Pre-Task Plan",
                    Sections = new List<Section>(),
                    Summary = "Wrapped"
                };
            }
        }
        // store in database for log/review
        input.GeneratedPtpJson = generatedContentForSql;
        await _context.SaveChangesAsync();
        var responseObj = new
        {
            inputId = input.Id,
            ptpDocument = structuredPtp
        };
        
        return CreatedAtAction(nameof(CreatePreTaskPlan), new { id = input.Id }, responseObj);
    }

    private async Task<string> CallAzureOpenAiAsync(string prompt)
    {
        string apiKey = _configuration["EHS-KEY1"] ?? throw new InvalidOperationException();
        string endpoint = _configuration["AzureOpenAI:Endpoint"] ?? throw new InvalidOperationException();
        string deploymentName = _configuration["AzureOpenAI:DeploymentName"] ?? throw new InvalidOperationException();
        string apiVersion = _configuration["AzureOpenAI:ApiVersion"] ?? throw new InvalidOperationException();

        var requestBody = new
        {
            messages = new object[]
            {
                new {role = "system", content = "You are an Environmental Health and Safety expert versed in OSHA regulations. Your output will always be structured JSON, you always return complete JSON objects."},
                new { role = "user", content = prompt }
            },
            max_tokens = 650,
            temperature = 0.7
        };

        string requestJson = JsonConvert.SerializeObject(requestBody);
        var content = new StringContent(requestJson, Encoding.UTF8, "application/json");
        
        // set API Key header
        _httpClient.DefaultRequestHeaders.Clear();
        _httpClient.DefaultRequestHeaders.Add("api-key", apiKey);
        
        // URL request
        string requestUrl = $"{endpoint}openai/deployments/{deploymentName}/completions?api-version={apiVersion}";

        HttpResponseMessage response = await _httpClient.PostAsync(requestUrl, content);
        response.EnsureSuccessStatusCode();
        string responseJson = await response.Content.ReadAsStringAsync();
        System.Diagnostics.Debug.WriteLine("Full AI response: " + responseJson);

        return responseJson;
    }
}