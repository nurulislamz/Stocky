using Microsoft.EntityFrameworkCore;
using stockyapi.Requests;
using stockymodels.models;
using stockyapi.Responses;

namespace stockyapi.Services;

public interface IPortfolioService
{
    public Task<PortfolioModel?> FetchUserPortfolio();
    public GetPortfolioResponse CreatePortfolioResponse(PortfolioModel? portfolio);
    public Task<BuyTickerResponse> BuyTickerPortfolio(BuyTickerRequest request);
    public Task<SellTickerResponse> SellTickerPortfolio(SellTickerRequest request);
}