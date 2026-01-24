using stockyapi.Application.Funds.CommandAndQueries;
using stockyapi.Middleware;
using stockyapi.Repository.Funds.Types;

namespace stockyapi.Repository.Funds;

public interface IFundsRepository
{
    // Fund Operations
    Task<Result<PortfolioBalances>> GetFundsAsync(Guid userId, CancellationToken cancellationToken);

    Task<Result<PortfolioBalances>> ApplyFundTransactionAsync(BaseFundCommand command, CancellationToken cancellationToken);
}