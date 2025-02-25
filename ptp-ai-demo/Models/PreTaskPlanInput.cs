using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace ptp_ai_demo.Models;

public class PreTaskPlanInput
{
    // Primary Key for SQL
    [Key]
    public int Id { get; set; }
    // DataAnnotations.Required binds field validation to incoming HTTP requests
    [Required(ErrorMessage = "Work area is required.")]
    public string WorkArea { get; set; }
    
    [Required(ErrorMessage = "Activity description is required")]
    public string ActivityDescription { get; set; }
    // Generated PTP as JSON for logging and review, also for front end field population
    [Column(TypeName = "nvarchar(MAX)")]
    public string? GeneratedPtpJson { get; set; }
    
    public string? SignedBy { get; set; }
    public string? Status { get; set; }
} 