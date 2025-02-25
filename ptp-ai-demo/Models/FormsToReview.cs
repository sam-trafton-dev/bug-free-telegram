using System.ComponentModel.DataAnnotations;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace ptp_ai_demo.Models;

public class FormsToReview
{
    [Key]
    public int Id { get; set; }
    
    //Foreign key ref to original generation
    public int PreTaskPlanId { get; set;}
    
    public string SignedBy { get; set;}
    
    [Column(TypeName = "nvarchar(MAX)")]
    public string UpdatedGeneratedJson { get; set;}
    
    public string Status { get; set;}
    
    public DateTime SubmittedAt { get; set; }
    
}