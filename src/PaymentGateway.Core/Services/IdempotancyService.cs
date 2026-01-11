using Microsoft.Extensions.Logging;

namespace PaymentGateway.Core.Services;

// TODO: HybridCache
public class IdempotancyService(ILogger<IdempotancyService> logger)
{
    private readonly HashSet<string> _idempotancyCache = [];

    public bool IfIdempotancyCheckFailed(string idempotancyId)
    {
        if (_idempotancyCache.Contains(idempotancyId))
        {
            logger.LogError("Idempotancy {IdempotancyId} has been already processed", idempotancyId);
            return true;
        }

        _idempotancyCache.Add(idempotancyId);

        return false;
    }
}
