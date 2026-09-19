using Opus127.Dengue.Api.Contracts;

namespace Opus127.Dengue.Api.Services;

public interface IDengueQueryService
{
    Task<DengueWeekResponse?> GetByWeekAsync(int ew, int ey, CancellationToken cancellationToken);
}
