using Microsoft.AspNetCore.Mvc;
using Opus127.Dengue.Api.Contracts;
using Opus127.Dengue.Api.Services;

namespace Opus127.Dengue.Api.Controllers;

[ApiController]
[Route("api/dengue")]
public sealed class DengueController : ControllerBase
{
    private readonly IDengueQueryService _query;
    private readonly IDengueSyncService _sync;

    public DengueController(IDengueQueryService query, IDengueSyncService sync)
    {
        _query = query;
        _sync = sync;
    }

    [HttpGet]
    public async Task<ActionResult<DengueWeekResponse>> Get(
        [FromQuery] int ew,
        [FromQuery] int ey,
        CancellationToken cancellationToken)
    {
        try
        {
            var response = await _query.GetByWeekAsync(ew, ey, cancellationToken);
            if (response is null)
                return NotFound();

            return Ok(response);
        }
        catch (ArgumentOutOfRangeException)
        {
            return BadRequest();
        }
    }

    [HttpPost("sync")]
    public async Task<ActionResult<SyncResponse>> Sync(CancellationToken cancellationToken)
    {
        try
        {
            var upserted = await _sync.SyncLastSixMonthsAsync(cancellationToken);
            return Ok(new SyncResponse(upserted));
        }
        catch (HttpRequestException)
        {
            return StatusCode(StatusCodes.Status502BadGateway);
        }
    }
}
