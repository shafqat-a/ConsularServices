using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FrameworkQ.ConsularServices.Forms;

/// <summary>
/// Stores a single submission/response for a form.
/// </summary>
public class FormResponse
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long FormResponseId { get; set; }

    [ForeignKey(nameof(FormDefinition))]
    public int FormDefinitionId { get; set; }
    public FormDefinition? FormDefinition { get; set; }

    /// <summary>
    /// Raw response data captured from SurveyJS (JSON serialized object).
    /// </summary>
    [Column(TypeName = "text")]
    public string ResponseJson { get; set; } = string.Empty;

    /// <summary>
    /// Optional user email / identifier submitting the form.
    /// </summary>
    [MaxLength(256)]
    public string? SubmittedBy { get; set; }

    public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;
}
