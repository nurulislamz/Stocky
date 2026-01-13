using stockyapi.Application.Portfolio;
using stockyapi.Middleware;
using stockymodels.models;

namespace stockyapi.Repository.Portfolio;

public interface IFundsRepository
{
    // Fund Operations
    Task<Result<PortfolioBalances>> GetFundsAsync(Guid userId, CancellationToken cancellationToken);

    Task<Result<PortfolioBalances>> ApplyFundTransactionAsync(BaseFundCommand command, CancellationToken cancellationToken);
}