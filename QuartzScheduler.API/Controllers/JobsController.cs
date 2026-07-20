using Microsoft.AspNetCore.Mvc;
using QuartzScheduler.API.Services;
using QuartzScheduler.Shared.DTOs.Jobs;

namespace QuartzScheduler.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class JobsController(IJobService jobService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<JobSummaryResponse>>> GetAll(CancellationToken ct)
    {
        var jobs = await jobService.GetAllAsync(ct);
        return Ok(jobs);
    }

    [HttpGet("email/{id:int}")]
    public async Task<ActionResult<EmailJobResponse>> GetEmailById(int id, CancellationToken ct)
    {
        var job = await jobService.GetEmailJobByIdAsync(id, ct);

        if (job is null)
        {
            return NotFound(new { message = "Email job not found." });
        }

        return Ok(job);
    }

    [HttpGet("http/{id:int}")]
    public async Task<ActionResult<HttpJobResponse>> GetHttpById(int id, CancellationToken ct)
    {
        var job = await jobService.GetHttpJobByIdAsync(id, ct);

        if (job is null)
        {
            return NotFound(new { message = "Http job not found." });
        }

        return Ok(job);
    }

    [HttpPost("email")]
    public async Task<ActionResult<EmailJobResponse>> CreateEmail(CreateEmailJobRequest request, CancellationToken ct)
    {
        var created = await jobService.CreateEmailJobAsync(request, ct);

        // 201 Created + Location header
        return CreatedAtAction(nameof(GetEmailById), new { id = created.Id }, created);
    }

    [HttpPost("http")]
    public async Task<ActionResult<HttpJobResponse>> CreateHttp(CreateHttpJobRequest request, CancellationToken ct)
    {
        var created = await jobService.CreateHttpJobAsync(request, ct);

        return CreatedAtAction(nameof(GetHttpById), new { id = created.Id }, created);
    }

    [HttpPut("email/{id:int}")]
    public async Task<ActionResult<EmailJobResponse>> UpdateEmail(int id, UpdateEmailJobRequest request, CancellationToken ct)
    {
        var updated = await jobService.UpdateEmailJobAsync(id, request, ct);

        if (updated is null)
        {
            return NotFound(new { message = "Email job not found." });
        }

        return Ok(updated);
    }

    [HttpPut("http/{id:int}")]
    public async Task<ActionResult<HttpJobResponse>> UpdateHttp(int id, UpdateHttpJobRequest request, CancellationToken ct)
    {
        var updated = await jobService.UpdateHttpJobAsync(id, request, ct);

        if (updated is null)
        {
            return NotFound(new { message = "Http job not found." });
        }

        return Ok(updated);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        var deleted = await jobService.DeleteAsync(id, ct);

        if (!deleted)
        {
            return NotFound(new { message = "Job not found." });
        }

        return NoContent();   // 204, nothing to return
    }
}