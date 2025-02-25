using Microsoft.EntityFrameworkCore;
using ptp_ai_demo.Models;

namespace ptp_ai_demo.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) :
        base(options)
    {
        
    }
    
    public DbSet<User> Users { get; set; }
    public DbSet<PreTaskPlanInput> PreTaskPlanInputs { get; set; }
    public DbSet<FormsToReview> FormsToReview { get; set;}
    
}