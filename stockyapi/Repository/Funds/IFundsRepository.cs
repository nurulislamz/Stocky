using stockyapi.Application.Commands.Funds;
using stockyapi.Repository.Funds.Types;

namespace stockyapi.Repository.Funds;

public interface IFundsRepository
{
    Task<PortfolioBalances> GetFundsAsync(Guid userId, CancellationToken cancellationToken);
    Task<PortfolioBalances> DepositFundsAsync(Guid userId, DepositFundsCommand command, CancellationToken cancellationToken);
    Task<PortfolioBalances> WithdrawFundsAsync(Guid userId, WithdrawFundsCommand command, CancellationToken cancellationToken);
}