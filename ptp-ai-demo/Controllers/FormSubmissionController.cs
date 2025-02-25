using Microsoft.AspNetCore.Mvc;
using ptp_ai_demo.Data;
using ptp_ai_demo.Models;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ptp_ai_demo.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FormSubmissionController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public FormSubmissionController(ApplicationDbContext context)
    {
        _context = context;
    }

    // POST: api/FormSubmission/{id:int}
    [HttpPost("{id}")]
    public async Task<IActionResult> SignAndSubmitForm(int id, [FromBody] SignSubmitModel model)
    {
        try
        {
            var form = await _context.PreTaskPlanInputs.FindAsync(id);
            if (form == null)
            {
                return NotFound(new { error = $"Form with id {id} not found." });
            }

            var stringifiedJson = JsonConvert.SerializeObject(model.UpdatedGeneratedJson);

            var reviewForm = new FormsToReview
            {
                PreTaskPlanId = form.Id,
                SignedBy = model.SignedBy,
                UpdatedGeneratedJson = stringifiedJson,
                Status = model.Status,
                SubmittedAt = DateTime.UtcNow
            };

            _context.FormsToReview.Add(reviewForm);

            // Optionally update the original form's status
            form.Status = "submitted";
            form.SignedBy = model.SignedBy;

            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(SignAndSubmitForm), new { id = reviewForm.Id }, new { message = "Form signed and submitted for review successfully." });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }
}


public class SignSubmitModel
{
    public string SignedBy { get; set; }
    public string Status { get; set; }
    public string UpdatedGeneratedJson { get; set; } // Checking changes between employee edits and AI suggestions can highlight people removing hazards
}
