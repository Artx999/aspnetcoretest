using System.ComponentModel.DataAnnotations;

namespace assignment_4.Models;

public class Blog
{
    public Blog() {}
    
    public int Id { get; set; }
    
    [Required]
    public string Title { get; set; } = String.Empty;
    
    [Required]
    public string Summary { get; set; } = String.Empty;
    
    [Required]
    public string Content { get; set; } = String.Empty;
    
    [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}")]
    public DateTime Timestamp { get; set; } = DateTime.Now;
    
    // Navigation properties
    public Guid ApplicationUserId { get; set; }
    public ApplicationUser ApplicationUser { get; set; }
}