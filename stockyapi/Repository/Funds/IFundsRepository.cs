using stockyapi.Repository.Funds.Types;

namespace stockyapi.Repository.Funds;

public interface IFundsRepository
{
    // Fund Operations
    Task<PortfolioBalances> GetFundsAsync(Guid userId, CancellationToken cancellationToken);
    Task<PortfolioBalances> DepositFundsAsync(Guid userId, Guid portfolioId, decimal cashDelta,  CancellationToken cancellationToken);
    Task<PortfolioBalances> WithdrawFundsAsync(Guid userId, Guid portfolioId, decimal cashDelta, CancellationToken cancellationToken);
}