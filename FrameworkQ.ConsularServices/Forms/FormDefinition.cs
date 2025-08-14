using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FrameworkQ.ConsularServices.Forms;

/// <summary>
/// Stores a SurveyJS form definition JSON plus metadata.
/// </summary>
public class FormDefinition
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int FormDefinitionId { get; set; }

    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Optional unique slug for routing.
    /// </summary>
    [MaxLength(200)]
    public string? Slug { get; set; }

    /// <summary>
    /// Raw Survey JSON definition.
    /// </summary>
    [Column(TypeName = "text")]
    public string Json { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;

    public int Version { get; set; } = 1;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
