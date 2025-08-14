using FrameworkQ.ConsularServices.Data;
using FrameworkQ.ConsularServices.Forms;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FrameworkQ.ConsularServices.Web.Controllers;

[ApiController]
[Route("api/forms")] // /api/forms
public class FormController : ControllerBase
{
    private readonly ConsularDbContext _db;

    public FormController(ConsularDbContext db)
    {
        _db = db;
    }

    // GET: /api/forms/definitions
    [HttpGet("definitions")]
    public async Task<IActionResult> GetDefinitions()
    {
        var defs = await _db.FormDefinitions.OrderByDescending(f => f.UpdatedAt).ToListAsync();
        return Ok(defs);
    }

    // GET: /api/forms/definitions/{id}
    [HttpGet("definitions/{id:int}")]
    public async Task<IActionResult> GetDefinition(int id)
    {
        var def = await _db.FormDefinitions.FindAsync(id);
        if (def == null) return NotFound();
        return Ok(def);
    }

    // POST: /api/forms/definitions (create or update if id present)
    [HttpPost("definitions")]
    public async Task<IActionResult> UpsertDefinition([FromBody] FormDefinition dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Name))
            return BadRequest("Name is required");

        FormDefinition entity;
        if (dto.FormDefinitionId > 0)
        {
            entity = await _db.FormDefinitions.FindAsync(dto.FormDefinitionId) ?? new FormDefinition();
            if (entity.FormDefinitionId == 0)
                _db.FormDefinitions.Add(entity);
        }
        else
        {
            entity = new FormDefinition();
            _db.FormDefinitions.Add(entity);
        }

        entity.Name = dto.Name;
        entity.Slug = dto.Slug;
        entity.Json = dto.Json;
        entity.IsActive = dto.IsActive;
        entity.Version = dto.Version == 0 ? entity.Version : dto.Version;
        entity.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();
        return Ok(entity);
    }

    // POST: /api/forms/{definitionId}/responses
    [HttpPost("{definitionId:int}/responses")]
    public async Task<IActionResult> SubmitResponse(int definitionId, [FromBody] object response)
    {
        var def = await _db.FormDefinitions.FindAsync(definitionId);
        if (def == null) return NotFound("Form definition not found");

        var resp = new FormResponse
        {
            FormDefinitionId = definitionId,
            ResponseJson = response?.ToString() ?? "{}",
            SubmittedBy = User?.Identity?.Name
        };
        _db.FormResponses.Add(resp);
        await _db.SaveChangesAsync();
        return Ok(new { resp.FormResponseId });
    }

    // GET: /api/forms/{definitionId}/responses (basic analytics)
    [HttpGet("{definitionId:int}/responses")]
    public async Task<IActionResult> ListResponses(int definitionId)
    {
        var list = await _db.FormResponses
            .Where(r => r.FormDefinitionId == definitionId)
            .OrderByDescending(r => r.SubmittedAt)
            .Take(200)
            .ToListAsync();
        return Ok(list);
    }
}
