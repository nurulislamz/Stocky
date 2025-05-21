using Microsoft.EntityFrameworkCore;
using stockymodels.models;
using stockyapi.Responses;

namespace stockyapi.Services;

public interface IPortfolioService
{
    public Task<GetPortfolioResponse> GetUserPortfolio();
}