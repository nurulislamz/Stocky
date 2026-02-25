using System.Text.Json.Serialization;
// ReSharper disable InconsistentNaming

namespace stockyapi.Services.YahooFinance.Types;

// ------------------------------------------------------------
// fundamentalsTimeSeries  (JSR: time series for many financial types; huge surface)
// Yahoo API returns: { "timeseries": { "result": [...], "error": null } }
// ------------------------------------------------------------

/// <summary>
/// Root response wrapper for the fundamentals timeseries endpoint.
/// </summary>
public sealed class FundamentalsTimeSeriesResponse : YahooFinanceDto
{
    [JsonPropertyName("timeseries")]
    public FundamentalsTimeseries Timeseries { get; set; } = null!;
}

/// <summary>
/// Timeseries wrapper containing the result array.
/// </summary>
public sealed class FundamentalsTimeseries : YahooFinanceDto
{
    [JsonPropertyName("result")]
    public List<FundamentalsTimeSeriesResultItem> Result { get; set; } = [];

    [JsonPropertyName("error")]
    public object? Error { get; set; }
}

/// <summary>
/// Individual result item from the timeseries endpoint.
/// Each item represents a single metric type with its values over time.
/// </summary>
public sealed class FundamentalsTimeSeriesResultItem : YahooFinanceDto
{
    [JsonPropertyName("meta")]
    public FundamentalsTimeSeriesMeta Meta { get; set; } = new();

    [JsonPropertyName("timestamp")]
    public List<long>? Timestamp { get; set; }
}

/// <summary>
/// Metadata for a fundamentals time series result.
/// </summary>
public sealed class FundamentalsTimeSeriesMeta : YahooFinanceDto
{
    [JsonPropertyName("symbol")]
    public List<string> Symbol { get; set; } = [];

    [JsonPropertyName("type")]
    public List<string> Type { get; set; } = [];
}

public sealed class FundamentalsTimeSeriesResults : List<FundamentalsTimeSeriesResult> { }

public sealed class FundamentalsTimeSeriesResult : YahooFinanceDto
{
    [JsonPropertyName("AmortizationAmortizationCashFlow")]
    public decimal? AmortizationAmortizationCashFlow { get; set; }

    [JsonPropertyName("EBIT")]
    public decimal? Ebit { get; set; }

    [JsonPropertyName("EBITDA")]
    public decimal? Ebitda { get; set; }

    [JsonPropertyName("TYPE")]
    public string Type { get; set; } = null!;

    [JsonPropertyName("accountsPayable")]
    public decimal? AccountsPayable { get; set; }

    [JsonPropertyName("accountsReceivable")]
    public decimal? AccountsReceivable { get; set; }

    [JsonPropertyName("accruedInterestReceivable")]
    public decimal? AccruedInterestReceivable { get; set; }

    [JsonPropertyName("accumulatedDepreciation")]
    public decimal? AccumulatedDepreciation { get; set; }

    [JsonPropertyName("additionalPaidInCapital")]
    public decimal? AdditionalPaidInCapital { get; set; }

    [JsonPropertyName("adjustedGeographySegmentData")]
    public decimal? AdjustedGeographySegmentData { get; set; }

    [JsonPropertyName("advanceFromFederalHomeLoanBanks")]
    public decimal? AdvanceFromFederalHomeLoanBanks { get; set; }

    [JsonPropertyName("allTaxesPaid")]
    public decimal? AllTaxesPaid { get; set; }

    [JsonPropertyName("allowanceForDoubtfulAccountsReceivable")]
    public decimal? AllowanceForDoubtfulAccountsReceivable { get; set; }

    [JsonPropertyName("allowanceForLoansAndLeaseLosses")]
    public decimal? AllowanceForLoansAndLeaseLosses { get; set; }

    [JsonPropertyName("allowanceForNotesReceivable")]
    public decimal? AllowanceForNotesReceivable { get; set; }

    [JsonPropertyName("amortization")]
    public decimal? Amortization { get; set; }

    [JsonPropertyName("amortizationCashFlow")]
    public decimal? AmortizationCashFlow { get; set; }

    [JsonPropertyName("amortizationOfFinancingCostsAndDiscounts")]
    public decimal? AmortizationOfFinancingCostsAndDiscounts { get; set; }

    [JsonPropertyName("amortizationOfIntangibles")]
    public decimal? AmortizationOfIntangibles { get; set; }

    [JsonPropertyName("amortizationOfIntangiblesIncomeStatement")]
    public decimal? AmortizationOfIntangiblesIncomeStatement { get; set; }

    [JsonPropertyName("amortizationOfSecurities")]
    public decimal? AmortizationOfSecurities { get; set; }

    [JsonPropertyName("assetImpairmentCharge")]
    public decimal? AssetImpairmentCharge { get; set; }

    [JsonPropertyName("assetsHeldForSale")]
    public decimal? AssetsHeldForSale { get; set; }

    [JsonPropertyName("assetsHeldForSaleCurrent")]
    public decimal? AssetsHeldForSaleCurrent { get; set; }

    [JsonPropertyName("availableForSaleSecurities")]
    public decimal? AvailableForSaleSecurities { get; set; }

    [JsonPropertyName("averageDilutionEarnings")]
    public decimal? AverageDilutionEarnings { get; set; }

    [JsonPropertyName("bankIndebtedness")]
    public decimal? BankIndebtedness { get; set; }

    [JsonPropertyName("bankOwnedLifeInsurance")]
    public decimal? BankOwnedLifeInsurance { get; set; }

    [JsonPropertyName("basicAccountingChange")]
    public decimal? BasicAccountingChange { get; set; }

    [JsonPropertyName("basicAverageShares")]
    public decimal? BasicAverageShares { get; set; }

    [JsonPropertyName("basicContinuousOperations")]
    public decimal? BasicContinuousOperations { get; set; }

    [JsonPropertyName("basicDiscontinuousOperations")]
    public decimal? BasicDiscontinuousOperations { get; set; }

    [JsonPropertyName("basicEPS")]
    public decimal? BasicEPS { get; set; }

    [JsonPropertyName("basicEPSOtherGainsLosses")]
    public decimal? BasicEPSOtherGainsLosses { get; set; }

    [JsonPropertyName("basicExtraordinary")]
    public decimal? BasicExtraordinary { get; set; }

    [JsonPropertyName("beginningCashPosition")]
    public decimal? BeginningCashPosition { get; set; }

    [JsonPropertyName("buildingsAndImprovements")]
    public decimal? BuildingsAndImprovements { get; set; }

    [JsonPropertyName("capitalExpenditure")]
    public decimal? CapitalExpenditure { get; set; }

    [JsonPropertyName("capitalExpenditureReported")]
    public decimal? CapitalExpenditureReported { get; set; }

    [JsonPropertyName("capitalLeaseObligations")]
    public decimal? CapitalLeaseObligations { get; set; }

    [JsonPropertyName("capitalStock")]
    public decimal? CapitalStock { get; set; }

    [JsonPropertyName("cashAndCashEquivalents")]
    public decimal? CashAndCashEquivalents { get; set; }

    [JsonPropertyName("cashAndDueFromBanks")]
    public decimal? CashAndDueFromBanks { get; set; }

    [JsonPropertyName("cashCashEquivalentsAndFederalFundsSold")]
    public decimal? CashCashEquivalentsAndFederalFundsSold { get; set; }

    [JsonPropertyName("cashCashEquivalentsAndShortTermInvestments")]
    public decimal? CashCashEquivalentsAndShortTermInvestments { get; set; }

    [JsonPropertyName("cashDividendsPaid")]
    public decimal? CashDividendsPaid { get; set; }

    [JsonPropertyName("cashEquivalents")]
    public decimal? CashEquivalents { get; set; }

    [JsonPropertyName("cashFinancial")]
    public decimal? CashFinancial { get; set; }

    [JsonPropertyName("cashFlowFromContinuingFinancingActivities")]
    public decimal? CashFlowFromContinuingFinancingActivities { get; set; }

    [JsonPropertyName("cashFlowFromContinuingInvestingActivities")]
    public decimal? CashFlowFromContinuingInvestingActivities { get; set; }

    [JsonPropertyName("cashFlowFromContinuingOperatingActivities")]
    public decimal? CashFlowFromContinuingOperatingActivities { get; set; }

    [JsonPropertyName("cashFlowFromDiscontinuedOperation")]
    public decimal? CashFlowFromDiscontinuedOperation { get; set; }

    [JsonPropertyName("cashFlowsfromusedinOperatingActivitiesDirect")]
    public decimal? CashFlowsfromusedinOperatingActivitiesDirect { get; set; }

    [JsonPropertyName("cashFromDiscontinuedFinancingActivities")]
    public decimal? CashFromDiscontinuedFinancingActivities { get; set; }

    [JsonPropertyName("cashFromDiscontinuedInvestingActivities")]
    public decimal? CashFromDiscontinuedInvestingActivities { get; set; }

    [JsonPropertyName("cashFromDiscontinuedOperatingActivities")]
    public decimal? CashFromDiscontinuedOperatingActivities { get; set; }

    [JsonPropertyName("cashPaymentsforDepositsbyBanksandCustomers")]
    public decimal? CashPaymentsforDepositsbyBanksandCustomers { get; set; }

    [JsonPropertyName("cashPaymentsforLoans")]
    public decimal? CashPaymentsforLoans { get; set; }

    [JsonPropertyName("cashReceiptsfromDepositsbyBanksandCustomers")]
    public decimal? CashReceiptsfromDepositsbyBanksandCustomers { get; set; }

    [JsonPropertyName("cashReceiptsfromFeesandCommissions")]
    public decimal? CashReceiptsfromFeesandCommissions { get; set; }

    [JsonPropertyName("cashReceiptsfromLoans")]
    public decimal? CashReceiptsfromLoans { get; set; }

    [JsonPropertyName("cashReceiptsfromSecuritiesRelatedActivities")]
    public decimal? CashReceiptsfromSecuritiesRelatedActivities { get; set; }

    [JsonPropertyName("cashReceiptsfromTaxRefunds")]
    public decimal? CashReceiptsfromTaxRefunds { get; set; }

    [JsonPropertyName("changeInAccountPayable")]
    public decimal? ChangeInAccountPayable { get; set; }

    [JsonPropertyName("changeInAccruedExpense")]
    public decimal? ChangeInAccruedExpense { get; set; }

    [JsonPropertyName("changeInDeferredCharges")]
    public decimal? ChangeInDeferredCharges { get; set; }

    [JsonPropertyName("changeInDividendPayable")]
    public decimal? ChangeInDividendPayable { get; set; }

    [JsonPropertyName("changeInFederalFundsAndSecuritiesSoldForRepurchase")]
    public decimal? ChangeInFederalFundsAndSecuritiesSoldForRepurchase { get; set; }

    [JsonPropertyName("changeInIncomeTaxPayable")]
    public decimal? ChangeInIncomeTaxPayable { get; set; }

    [JsonPropertyName("changeInInterestPayable")]
    public decimal? ChangeInInterestPayable { get; set; }

    [JsonPropertyName("changeInInventory")]
    public decimal? ChangeInInventory { get; set; }

    [JsonPropertyName("changeInLoans")]
    public decimal? ChangeInLoans { get; set; }

    [JsonPropertyName("changeInOtherCurrentAssets")]
    public decimal? ChangeInOtherCurrentAssets { get; set; }

    [JsonPropertyName("changeInOtherCurrentLiabilities")]
    public decimal? ChangeInOtherCurrentLiabilities { get; set; }

    [JsonPropertyName("changeInOtherWorkingCapital")]
    public decimal? ChangeInOtherWorkingCapital { get; set; }

    [JsonPropertyName("changeInPayable")]
    public decimal? ChangeInPayable { get; set; }

    [JsonPropertyName("changeInPayablesAndAccruedExpense")]
    public decimal? ChangeInPayablesAndAccruedExpense { get; set; }

    [JsonPropertyName("changeInPrepaidAssets")]
    public decimal? ChangeInPrepaidAssets { get; set; }

    [JsonPropertyName("changeInReceivables")]
    public decimal? ChangeInReceivables { get; set; }

    [JsonPropertyName("changeInTaxPayable")]
    public decimal? ChangeInTaxPayable { get; set; }

    [JsonPropertyName("changeInWorkingCapital")]
    public decimal? ChangeInWorkingCapital { get; set; }

    [JsonPropertyName("changesInAccountReceivables")]
    public decimal? ChangesInAccountReceivables { get; set; }

    [JsonPropertyName("changesInCash")]
    public decimal? ChangesInCash { get; set; }

    [JsonPropertyName("classesofCashPayments")]
    public decimal? ClassesofCashPayments { get; set; }

    [JsonPropertyName("classesofCashReceiptsfromOperatingActivities")]
    public decimal? ClassesofCashReceiptsfromOperatingActivities { get; set; }

    [JsonPropertyName("commercialLoan")]
    public decimal? CommercialLoan { get; set; }

    [JsonPropertyName("commercialPaper")]
    public decimal? CommercialPaper { get; set; }

    [JsonPropertyName("commonStock")]
    public decimal? CommonStock { get; set; }

    [JsonPropertyName("commonStockDividendPaid")]
    public decimal? CommonStockDividendPaid { get; set; }

    [JsonPropertyName("commonStockEquity")]
    public decimal? CommonStockEquity { get; set; }

    [JsonPropertyName("commonStockIssuance")]
    public decimal? CommonStockIssuance { get; set; }

    [JsonPropertyName("commonStockPayments")]
    public decimal? CommonStockPayments { get; set; }

    [JsonPropertyName("constructionInProgress")]
    public decimal? ConstructionInProgress { get; set; }

    [JsonPropertyName("consumerLoan")]
    public decimal? ConsumerLoan { get; set; }

    [JsonPropertyName("continuingAndDiscontinuedBasicEPS")]
    public decimal? ContinuingAndDiscontinuedBasicEPS { get; set; }

    [JsonPropertyName("continuingAndDiscontinuedDilutedEPS")]
    public decimal? ContinuingAndDiscontinuedDilutedEPS { get; set; }

    [JsonPropertyName("costOfRevenue")]
    public decimal? CostOfRevenue { get; set; }

    [JsonPropertyName("creditCard")]
    public decimal? CreditCard { get; set; }

    [JsonPropertyName("creditLossesProvision")]
    public decimal? CreditLossesProvision { get; set; }

    [JsonPropertyName("currentAccruedExpenses")]
    public decimal? CurrentAccruedExpenses { get; set; }

    [JsonPropertyName("currentAssets")]
    public decimal? CurrentAssets { get; set; }

    [JsonPropertyName("currentCapitalLeaseObligation")]
    public decimal? CurrentCapitalLeaseObligation { get; set; }

    [JsonPropertyName("currentDebt")]
    public decimal? CurrentDebt { get; set; }

    [JsonPropertyName("currentDebtAndCapitalLeaseObligation")]
    public decimal? CurrentDebtAndCapitalLeaseObligation { get; set; }

    [JsonPropertyName("currentDeferredAssets")]
    public decimal? CurrentDeferredAssets { get; set; }

    [JsonPropertyName("currentDeferredLiabilities")]
    public decimal? CurrentDeferredLiabilities { get; set; }

    [JsonPropertyName("currentDeferredRevenue")]
    public decimal? CurrentDeferredRevenue { get; set; }

    [JsonPropertyName("currentDeferredTaxesAssets")]
    public decimal? CurrentDeferredTaxesAssets { get; set; }

    [JsonPropertyName("currentDeferredTaxesLiabilities")]
    public decimal? CurrentDeferredTaxesLiabilities { get; set; }

    [JsonPropertyName("currentLiabilities")]
    public decimal? CurrentLiabilities { get; set; }

    [JsonPropertyName("currentNotesPayable")]
    public decimal? CurrentNotesPayable { get; set; }

    [JsonPropertyName("currentProvisions")]
    public decimal? CurrentProvisions { get; set; }

    [JsonPropertyName("customerAcceptances")]
    public decimal? CustomerAcceptances { get; set; }

    [JsonPropertyName("customerAccounts")]
    public decimal? CustomerAccounts { get; set; }

    [JsonPropertyName("date")]
    [JsonConverter(typeof(UnixSecondsDateTimeOffsetConverter))]
    public DateTimeOffset Date { get; set; }

    [JsonPropertyName("decreaseinInterestBearingDepositsinBank")]
    public decimal? DecreaseinInterestBearingDepositsinBank { get; set; }

    [JsonPropertyName("deferredAssets")]
    public decimal? DeferredAssets { get; set; }

    [JsonPropertyName("deferredIncomeTax")]
    public decimal? DeferredIncomeTax { get; set; }

    [JsonPropertyName("deferredTax")]
    public decimal? DeferredTax { get; set; }

    [JsonPropertyName("deferredTaxAssets")]
    public decimal? DeferredTaxAssets { get; set; }

    [JsonPropertyName("definedPensionBenefit")]
    public decimal? DefinedPensionBenefit { get; set; }

    [JsonPropertyName("depletion")]
    public decimal? Depletion { get; set; }

    [JsonPropertyName("depletionIncomeStatement")]
    public decimal? DepletionIncomeStatement { get; set; }

    [JsonPropertyName("depositsbyBank")]
    public decimal? DepositsbyBank { get; set; }

    [JsonPropertyName("depreciation")]
    public decimal? Depreciation { get; set; }

    [JsonPropertyName("depreciationAmortizationDepletion")]
    public decimal? DepreciationAmortizationDepletion { get; set; }

    [JsonPropertyName("depreciationAmortizationDepletionIncomeStatement")]
    public decimal? DepreciationAmortizationDepletionIncomeStatement { get; set; }

    [JsonPropertyName("depreciationAndAmortization")]
    public decimal? DepreciationAndAmortization { get; set; }

    [JsonPropertyName("depreciationAndAmortizationInIncomeStatement")]
    public decimal? DepreciationAndAmortizationInIncomeStatement { get; set; }

    [JsonPropertyName("depreciationDepreciationIncomeStatement")]
    public decimal? DepreciationDepreciationIncomeStatement { get; set; }

    [JsonPropertyName("depreciationIncomeStatement")]
    public decimal? DepreciationIncomeStatement { get; set; }

    [JsonPropertyName("derivativeAssets")]
    public decimal? DerivativeAssets { get; set; }

    [JsonPropertyName("derivativeProductLiabilities")]
    public decimal? DerivativeProductLiabilities { get; set; }

    [JsonPropertyName("dilutedAccountingChange")]
    public decimal? DilutedAccountingChange { get; set; }

    [JsonPropertyName("dilutedAverageShares")]
    public decimal? DilutedAverageShares { get; set; }

    [JsonPropertyName("dilutedContinuousOperations")]
    public decimal? DilutedContinuousOperations { get; set; }

    [JsonPropertyName("dilutedDiscontinuousOperations")]
    public decimal? DilutedDiscontinuousOperations { get; set; }

    [JsonPropertyName("dilutedEPS")]
    public decimal? DilutedEPS { get; set; }

    [JsonPropertyName("dilutedEPSOtherGainsLosses")]
    public decimal? DilutedEPSOtherGainsLosses { get; set; }

    [JsonPropertyName("dilutedExtraordinary")]
    public decimal? DilutedExtraordinary { get; set; }

    [JsonPropertyName("dilutedNIAvailtoComStockholders")]
    public decimal? DilutedNIAvailtoComStockholders { get; set; }

    [JsonPropertyName("dividendIncome")]
    public decimal? DividendIncome { get; set; }

    [JsonPropertyName("dividendPaidCFO")]
    public decimal? DividendPaidCFO { get; set; }

    [JsonPropertyName("dividendPerShare")]
    public decimal? DividendPerShare { get; set; }

    [JsonPropertyName("dividendReceivedCFO")]
    public decimal? DividendReceivedCFO { get; set; }

    [JsonPropertyName("dividendsPaidDirect")]
    public decimal? DividendsPaidDirect { get; set; }

    [JsonPropertyName("dividendsPayable")]
    public decimal? DividendsPayable { get; set; }

    [JsonPropertyName("dividendsReceivedCFI")]
    public decimal? DividendsReceivedCFI { get; set; }

    [JsonPropertyName("dividendsReceivedDirect")]
    public decimal? DividendsReceivedDirect { get; set; }

    [JsonPropertyName("domesticSales")]
    public decimal? DomesticSales { get; set; }

    [JsonPropertyName("dueFromRelatedParties")]
    public decimal? DueFromRelatedParties { get; set; }

    [JsonPropertyName("duefromRelatedPartiesCurrent")]
    public decimal? DuefromRelatedPartiesCurrent { get; set; }

    [JsonPropertyName("duefromRelatedPartiesNonCurrent")]
    public decimal? DuefromRelatedPartiesNonCurrent { get; set; }

    [JsonPropertyName("duetoRelatedParties")]
    public decimal? DuetoRelatedParties { get; set; }

    [JsonPropertyName("duetoRelatedPartiesCurrent")]
    public decimal? DuetoRelatedPartiesCurrent { get; set; }

    [JsonPropertyName("duetoRelatedPartiesNonCurrent")]
    public decimal? DuetoRelatedPartiesNonCurrent { get; set; }

    [JsonPropertyName("earningsFromEquityInterest")]
    public decimal? EarningsFromEquityInterest { get; set; }

    [JsonPropertyName("earningsFromEquityInterestNetOfTax")]
    public decimal? EarningsFromEquityInterestNetOfTax { get; set; }

    [JsonPropertyName("earningsLossesFromEquityInvestments")]
    public decimal? EarningsLossesFromEquityInvestments { get; set; }

    [JsonPropertyName("effectOfExchangeRateChanges")]
    public decimal? EffectOfExchangeRateChanges { get; set; }

    [JsonPropertyName("employeeBenefits")]
    public decimal? EmployeeBenefits { get; set; }

    [JsonPropertyName("endCashPosition")]
    public decimal? EndCashPosition { get; set; }

    [JsonPropertyName("equipment")]
    public decimal? Equipment { get; set; }

    [JsonPropertyName("excessTaxBenefitFromStockBasedCompensation")]
    public decimal? ExcessTaxBenefitFromStockBasedCompensation { get; set; }

    [JsonPropertyName("exciseTaxes")]
    public decimal? ExciseTaxes { get; set; }

    [JsonPropertyName("explorationDevelopmentAndMineralPropertyLeaseExpenses")]
    public decimal? ExplorationDevelopmentAndMineralPropertyLeaseExpenses { get; set; }

    [JsonPropertyName("federalFundsPurchased")]
    public decimal? FederalFundsPurchased { get; set; }

    [JsonPropertyName("federalFundsPurchasedAndSecuritiesSoldUnderAgreementToRepurchase")]
    public decimal? FederalFundsPurchasedAndSecuritiesSoldUnderAgreementToRepurchase { get; set; }

    [JsonPropertyName("federalFundsSold")]
    public decimal? FederalFundsSold { get; set; }

    [JsonPropertyName("federalFundsSoldAndSecuritiesPurchaseUnderAgreementsToResell")]
    public decimal? FederalFundsSoldAndSecuritiesPurchaseUnderAgreementsToResell { get; set; }

    [JsonPropertyName("federalHomeLoanBankStock")]
    public decimal? FederalHomeLoanBankStock { get; set; }

    [JsonPropertyName("feesAndCommissions")]
    public decimal? FeesAndCommissions { get; set; }

    [JsonPropertyName("feesandCommissionExpense")]
    public decimal? FeesandCommissionExpense { get; set; }

    [JsonPropertyName("feesandCommissionIncome")]
    public decimal? FeesandCommissionIncome { get; set; }

    [JsonPropertyName("financialAssets")]
    public decimal? FinancialAssets { get; set; }

    [JsonPropertyName("financialAssetsDesignatedasFairValueThroughProfitorLossTotal")]
    public decimal? FinancialAssetsDesignatedasFairValueThroughProfitorLossTotal { get; set; }

    [JsonPropertyName("financialInstrumentsSoldUnderAgreementsToRepurchase")]
    public decimal? FinancialInstrumentsSoldUnderAgreementsToRepurchase { get; set; }

    [JsonPropertyName("financingCashFlow")]
    public decimal? FinancingCashFlow { get; set; }

    [JsonPropertyName("finishedGoods")]
    public decimal? FinishedGoods { get; set; }

    [JsonPropertyName("fixedAssetsRevaluationReserve")]
    public decimal? FixedAssetsRevaluationReserve { get; set; }

    [JsonPropertyName("foreclosedAssets")]
    public decimal? ForeclosedAssets { get; set; }

    [JsonPropertyName("foreignCurrencyTranslationAdjustments")]
    public decimal? ForeignCurrencyTranslationAdjustments { get; set; }

    [JsonPropertyName("foreignExchangeTradingGains")]
    public decimal? ForeignExchangeTradingGains { get; set; }

    [JsonPropertyName("foreignSales")]
    public decimal? ForeignSales { get; set; }

    [JsonPropertyName("freeCashFlow")]
    public decimal? FreeCashFlow { get; set; }

    [JsonPropertyName("gainLossOnInvestmentSecurities")]
    public decimal? GainLossOnInvestmentSecurities { get; set; }

    [JsonPropertyName("gainLossOnSaleOfBusiness")]
    public decimal? GainLossOnSaleOfBusiness { get; set; }

    [JsonPropertyName("gainLossOnSaleOfPPE")]
    public decimal? GainLossOnSaleOfPPE { get; set; }

    [JsonPropertyName("gainLossonSaleofAssets")]
    public decimal? GainLossonSaleofAssets { get; set; }

    [JsonPropertyName("gainOnSaleOfBusiness")]
    public decimal? GainOnSaleOfBusiness { get; set; }

    [JsonPropertyName("gainOnSaleOfPPE")]
    public decimal? GainOnSaleOfPPE { get; set; }

    [JsonPropertyName("gainOnSaleOfSecurity")]
    public decimal? GainOnSaleOfSecurity { get; set; }

    [JsonPropertyName("gainonSaleofInvestmentProperty")]
    public decimal? GainonSaleofInvestmentProperty { get; set; }

    [JsonPropertyName("gainonSaleofLoans")]
    public decimal? GainonSaleofLoans { get; set; }

    [JsonPropertyName("gainsLossesNotAffectingRetainedEarnings")]
    public decimal? GainsLossesNotAffectingRetainedEarnings { get; set; }

    [JsonPropertyName("generalAndAdministrativeExpense")]
    public decimal? GeneralAndAdministrativeExpense { get; set; }

    [JsonPropertyName("generalPartnershipCapital")]
    public decimal? GeneralPartnershipCapital { get; set; }

    [JsonPropertyName("goodwill")]
    public decimal? Goodwill { get; set; }

    [JsonPropertyName("goodwillAndOtherIntangibleAssets")]
    public decimal? GoodwillAndOtherIntangibleAssets { get; set; }

    [JsonPropertyName("grossAccountsReceivable")]
    public decimal? GrossAccountsReceivable { get; set; }

    [JsonPropertyName("grossLoan")]
    public decimal? GrossLoan { get; set; }

    [JsonPropertyName("grossNotesReceivable")]
    public decimal? GrossNotesReceivable { get; set; }

    [JsonPropertyName("grossPPE")]
    public decimal? GrossPPE { get; set; }

    [JsonPropertyName("grossProfit")]
    public decimal? GrossProfit { get; set; }

    [JsonPropertyName("hedgingAssetsCurrent")]
    public decimal? HedgingAssetsCurrent { get; set; }

    [JsonPropertyName("heldToMaturitySecurities")]
    public decimal? HeldToMaturitySecurities { get; set; }

    [JsonPropertyName("impairmentOfCapitalAssets")]
    public decimal? ImpairmentOfCapitalAssets { get; set; }

    [JsonPropertyName("incomeTaxPaidSupplementalData")]
    public decimal? IncomeTaxPaidSupplementalData { get; set; }

    [JsonPropertyName("incomeTaxPayable")]
    public decimal? IncomeTaxPayable { get; set; }

    [JsonPropertyName("incomefromAssociatesandOtherParticipatingInterests")]
    public decimal? IncomefromAssociatesandOtherParticipatingInterests { get; set; }

    [JsonPropertyName("increaseDecreaseInDeposit")]
    public decimal? IncreaseDecreaseInDeposit { get; set; }

    [JsonPropertyName("increaseinInterestBearingDepositsinBank")]
    public decimal? IncreaseinInterestBearingDepositsinBank { get; set; }

    [JsonPropertyName("insuranceAndClaims")]
    public decimal? InsuranceAndClaims { get; set; }

    [JsonPropertyName("interestBearingDepositsAssets")]
    public decimal? InterestBearingDepositsAssets { get; set; }

    [JsonPropertyName("interestBearingDepositsLiabilities")]
    public decimal? InterestBearingDepositsLiabilities { get; set; }

    [JsonPropertyName("interestExpense")]
    public decimal? InterestExpense { get; set; }

    [JsonPropertyName("interestExpenseForDeposit")]
    public decimal? InterestExpenseForDeposit { get; set; }

    [JsonPropertyName("interestExpenseForLongTermDebtAndCapitalSecurities")]
    public decimal? InterestExpenseForLongTermDebtAndCapitalSecurities { get; set; }

    [JsonPropertyName("interestExpenseForShortTermDebt")]
    public decimal? InterestExpenseForShortTermDebt { get; set; }

    [JsonPropertyName("interestExpenseNonOperating")]
    public decimal? InterestExpenseNonOperating { get; set; }

    [JsonPropertyName("interestIncome")]
    public decimal? InterestIncome { get; set; }

    [JsonPropertyName("interestIncomeAfterProvisionForLoanLoss")]
    public decimal? InterestIncomeAfterProvisionForLoanLoss { get; set; }

    [JsonPropertyName("interestIncomeFromDeposits")]
    public decimal? InterestIncomeFromDeposits { get; set; }

    [JsonPropertyName("interestIncomeFromLeases")]
    public decimal? InterestIncomeFromLeases { get; set; }

    [JsonPropertyName("interestIncomeFromLoans")]
    public decimal? InterestIncomeFromLoans { get; set; }

    [JsonPropertyName("interestIncomeFromLoansAndLease")]
    public decimal? InterestIncomeFromLoansAndLease { get; set; }

    [JsonPropertyName("interestIncomeFromSecurities")]
    public decimal? InterestIncomeFromSecurities { get; set; }

    [JsonPropertyName("interestIncomeNonOperating")]
    public decimal? InterestIncomeNonOperating { get; set; }

    [JsonPropertyName("interestPaidCFF")]
    public decimal? InterestPaidCFF { get; set; }

    [JsonPropertyName("interestPaidCFO")]
    public decimal? InterestPaidCFO { get; set; }

    [JsonPropertyName("interestPaidDirect")]
    public decimal? InterestPaidDirect { get; set; }

    [JsonPropertyName("interestPaidSupplementalData")]
    public decimal? InterestPaidSupplementalData { get; set; }

    [JsonPropertyName("interestPayable")]
    public decimal? InterestPayable { get; set; }

    [JsonPropertyName("interestReceivedCFI")]
    public decimal? InterestReceivedCFI { get; set; }

    [JsonPropertyName("interestReceivedCFO")]
    public decimal? InterestReceivedCFO { get; set; }

    [JsonPropertyName("interestReceivedDirect")]
    public decimal? InterestReceivedDirect { get; set; }

    [JsonPropertyName("interestandCommissionPaid")]
    public decimal? InterestandCommissionPaid { get; set; }

    [JsonPropertyName("inventoriesAdjustmentsAllowances")]
    public decimal? InventoriesAdjustmentsAllowances { get; set; }

    [JsonPropertyName("inventory")]
    public decimal? Inventory { get; set; }

    [JsonPropertyName("investedCapital")]
    public decimal? InvestedCapital { get; set; }

    [JsonPropertyName("investingCashFlow")]
    public decimal? InvestingCashFlow { get; set; }

    [JsonPropertyName("investmentBankingProfit")]
    public decimal? InvestmentBankingProfit { get; set; }

    [JsonPropertyName("investmentProperties")]
    public decimal? InvestmentProperties { get; set; }

    [JsonPropertyName("investmentinFinancialAssets")]
    public decimal? InvestmentinFinancialAssets { get; set; }

    [JsonPropertyName("investmentsAndAdvances")]
    public decimal? InvestmentsAndAdvances { get; set; }

    [JsonPropertyName("investmentsInOtherVenturesUnderEquityMethod")]
    public decimal? InvestmentsInOtherVenturesUnderEquityMethod { get; set; }

    [JsonPropertyName("investmentsinAssociatesatCost")]
    public decimal? InvestmentsinAssociatesatCost { get; set; }

    [JsonPropertyName("investmentsinJointVenturesatCost")]
    public decimal? InvestmentsinJointVenturesatCost { get; set; }

    [JsonPropertyName("investmentsinSubsidiariesatCost")]
    public decimal? InvestmentsinSubsidiariesatCost { get; set; }

    [JsonPropertyName("issuanceOfCapitalStock")]
    public decimal? IssuanceOfCapitalStock { get; set; }

    [JsonPropertyName("issuanceOfDebt")]
    public decimal? IssuanceOfDebt { get; set; }

    [JsonPropertyName("landAndImprovements")]
    public decimal? LandAndImprovements { get; set; }

    [JsonPropertyName("leases")]
    public decimal? Leases { get; set; }

    [JsonPropertyName("liabilitiesHeldforSaleNonCurrent")]
    public decimal? LiabilitiesHeldforSaleNonCurrent { get; set; }

    [JsonPropertyName("liabilitiesOfDiscontinuedOperations")]
    public decimal? LiabilitiesOfDiscontinuedOperations { get; set; }

    [JsonPropertyName("limitedPartnershipCapital")]
    public decimal? LimitedPartnershipCapital { get; set; }

    [JsonPropertyName("lineOfCredit")]
    public decimal? LineOfCredit { get; set; }

    [JsonPropertyName("loansHeldForSale")]
    public decimal? LoansHeldForSale { get; set; }

    [JsonPropertyName("loansReceivable")]
    public decimal? LoansReceivable { get; set; }

    [JsonPropertyName("longTermCapitalLeaseObligation")]
    public decimal? LongTermCapitalLeaseObligation { get; set; }

    [JsonPropertyName("longTermDebt")]
    public decimal? LongTermDebt { get; set; }

    [JsonPropertyName("longTermDebtAndCapitalLeaseObligation")]
    public decimal? LongTermDebtAndCapitalLeaseObligation { get; set; }

    [JsonPropertyName("longTermDebtIssuance")]
    public decimal? LongTermDebtIssuance { get; set; }

    [JsonPropertyName("longTermDebtPayments")]
    public decimal? LongTermDebtPayments { get; set; }

    [JsonPropertyName("longTermEquityInvestment")]
    public decimal? LongTermEquityInvestment { get; set; }

    [JsonPropertyName("longTermProvisions")]
    public decimal? LongTermProvisions { get; set; }

    [JsonPropertyName("lossonExtinguishmentofDebt")]
    public decimal? LossonExtinguishmentofDebt { get; set; }

    [JsonPropertyName("machineryFurnitureEquipment")]
    public decimal? MachineryFurnitureEquipment { get; set; }

    [JsonPropertyName("mineralProperties")]
    public decimal? MineralProperties { get; set; }

    [JsonPropertyName("minimumPensionLiabilities")]
    public decimal? MinimumPensionLiabilities { get; set; }

    [JsonPropertyName("minorityInterest")]
    public decimal? MinorityInterest { get; set; }

    [JsonPropertyName("minorityInterests")]
    public decimal? MinorityInterests { get; set; }

    [JsonPropertyName("moneyMarketInvestments")]
    public decimal? MoneyMarketInvestments { get; set; }

    [JsonPropertyName("mortgageLoan")]
    public decimal? MortgageLoan { get; set; }

    [JsonPropertyName("netBusinessPurchaseAndSale")]
    public decimal? NetBusinessPurchaseAndSale { get; set; }

    [JsonPropertyName("netCommonStockIssuance")]
    public decimal? NetCommonStockIssuance { get; set; }

    [JsonPropertyName("netDebt")]
    public decimal? NetDebt { get; set; }

    [JsonPropertyName("netForeignCurrencyExchangeGainLoss")]
    public decimal? NetForeignCurrencyExchangeGainLoss { get; set; }

    [JsonPropertyName("netIncome")]
    public decimal? NetIncome { get; set; }

    [JsonPropertyName("netIncomeCommonStockholders")]
    public decimal? NetIncomeCommonStockholders { get; set; }

    [JsonPropertyName("netIncomeContinuousOperations")]
    public decimal? NetIncomeContinuousOperations { get; set; }

    [JsonPropertyName("netIncomeDiscontinuousOperations")]
    public decimal? NetIncomeDiscontinuousOperations { get; set; }

    [JsonPropertyName("netIncomeExtraordinary")]
    public decimal? NetIncomeExtraordinary { get; set; }

    [JsonPropertyName("netIncomeFromContinuingAndDiscontinuedOperation")]
    public decimal? NetIncomeFromContinuingAndDiscontinuedOperation { get; set; }

    [JsonPropertyName("netIncomeFromContinuingOperationNetMinorityInterest")]
    public decimal? NetIncomeFromContinuingOperationNetMinorityInterest { get; set; }

    [JsonPropertyName("netIncomeFromContinuingOperations")]
    public decimal? NetIncomeFromContinuingOperations { get; set; }

    [JsonPropertyName("netIncomeFromTaxLossCarryforward")]
    public decimal? NetIncomeFromTaxLossCarryforward { get; set; }

    [JsonPropertyName("netIncomeIncludingNoncontrollingInterests")]
    public decimal? NetIncomeIncludingNoncontrollingInterests { get; set; }

    [JsonPropertyName("netIntangiblesPurchaseAndSale")]
    public decimal? NetIntangiblesPurchaseAndSale { get; set; }

    [JsonPropertyName("netInterestIncome")]
    public decimal? NetInterestIncome { get; set; }

    [JsonPropertyName("netInvestmentPropertiesPurchaseAndSale")]
    public decimal? NetInvestmentPropertiesPurchaseAndSale { get; set; }

    [JsonPropertyName("netInvestmentPurchaseAndSale")]
    public decimal? NetInvestmentPurchaseAndSale { get; set; }

    [JsonPropertyName("netIssuancePaymentsOfDebt")]
    public decimal? NetIssuancePaymentsOfDebt { get; set; }

    [JsonPropertyName("netLoan")]
    public decimal? NetLoan { get; set; }

    [JsonPropertyName("netLongTermDebtIssuance")]
    public decimal? NetLongTermDebtIssuance { get; set; }

    [JsonPropertyName("netNonOperatingInterestIncomeExpense")]
    public decimal? NetNonOperatingInterestIncomeExpense { get; set; }

    [JsonPropertyName("netOccupancyExpense")]
    public decimal? NetOccupancyExpense { get; set; }

    [JsonPropertyName("netOtherFinancingCharges")]
    public decimal? NetOtherFinancingCharges { get; set; }

    [JsonPropertyName("netOtherInvestingChanges")]
    public decimal? NetOtherInvestingChanges { get; set; }

    [JsonPropertyName("netPPE")]
    public decimal? NetPPE { get; set; }

    [JsonPropertyName("netPPEPurchaseAndSale")]
    public decimal? NetPPEPurchaseAndSale { get; set; }

    [JsonPropertyName("netPreferredStockIssuance")]
    public decimal? NetPreferredStockIssuance { get; set; }

    [JsonPropertyName("netProceedsPaymentForLoan")]
    public decimal? NetProceedsPaymentForLoan { get; set; }

    [JsonPropertyName("netShortTermDebtIssuance")]
    public decimal? NetShortTermDebtIssuance { get; set; }

    [JsonPropertyName("netTangibleAssets")]
    public decimal? NetTangibleAssets { get; set; }

    [JsonPropertyName("nonCurrentAccountsReceivable")]
    public decimal? NonCurrentAccountsReceivable { get; set; }

    [JsonPropertyName("nonCurrentAccruedExpenses")]
    public decimal? NonCurrentAccruedExpenses { get; set; }

    [JsonPropertyName("nonCurrentDeferredAssets")]
    public decimal? NonCurrentDeferredAssets { get; set; }

    [JsonPropertyName("nonCurrentDeferredLiabilities")]
    public decimal? NonCurrentDeferredLiabilities { get; set; }

    [JsonPropertyName("nonCurrentDeferredRevenue")]
    public decimal? NonCurrentDeferredRevenue { get; set; }

    [JsonPropertyName("nonCurrentDeferredTaxesAssets")]
    public decimal? NonCurrentDeferredTaxesAssets { get; set; }

    [JsonPropertyName("nonCurrentDeferredTaxesLiabilities")]
    public decimal? NonCurrentDeferredTaxesLiabilities { get; set; }

    [JsonPropertyName("nonCurrentNoteReceivables")]
    public decimal? NonCurrentNoteReceivables { get; set; }

    [JsonPropertyName("nonCurrentPensionAndOtherPostretirementBenefitPlans")]
    public decimal? NonCurrentPensionAndOtherPostretirementBenefitPlans { get; set; }

    [JsonPropertyName("nonCurrentPrepaidAssets")]
    public decimal? NonCurrentPrepaidAssets { get; set; }

    [JsonPropertyName("nonInterestBearingDeposits")]
    public decimal? NonInterestBearingDeposits { get; set; }

    [JsonPropertyName("nonInterestExpense")]
    public decimal? NonInterestExpense { get; set; }

    [JsonPropertyName("nonInterestIncome")]
    public decimal? NonInterestIncome { get; set; }

    [JsonPropertyName("normalizedBasicEPS")]
    public decimal? NormalizedBasicEPS { get; set; }

    [JsonPropertyName("normalizedDilutedEPS")]
    public decimal? NormalizedDilutedEPS { get; set; }

    [JsonPropertyName("normalizedEBITDA")]
    public decimal? NormalizedEBITDA { get; set; }

    [JsonPropertyName("normalizedIncome")]
    public decimal? NormalizedIncome { get; set; }

    [JsonPropertyName("notesReceivable")]
    public decimal? NotesReceivable { get; set; }

    [JsonPropertyName("occupancyAndEquipment")]
    public decimal? OccupancyAndEquipment { get; set; }

    [JsonPropertyName("operatingCashFlow")]
    public decimal? OperatingCashFlow { get; set; }

    [JsonPropertyName("operatingExpense")]
    public decimal? OperatingExpense { get; set; }

    [JsonPropertyName("operatingGainsLosses")]
    public decimal? OperatingGainsLosses { get; set; }

    [JsonPropertyName("operatingIncome")]
    public decimal? OperatingIncome { get; set; }

    [JsonPropertyName("operatingRevenue")]
    public decimal? OperatingRevenue { get; set; }

    [JsonPropertyName("operationAndMaintenance")]
    public decimal? OperationAndMaintenance { get; set; }

    [JsonPropertyName("ordinarySharesNumber")]
    public decimal? OrdinarySharesNumber { get; set; }

    [JsonPropertyName("otherAssets")]
    public decimal? OtherAssets { get; set; }

    [JsonPropertyName("otherCapitalStock")]
    public decimal? OtherCapitalStock { get; set; }

    [JsonPropertyName("otherCashAdjustmentInsideChangeinCash")]
    public decimal? OtherCashAdjustmentInsideChangeinCash { get; set; }

    [JsonPropertyName("otherCashAdjustmentOutsideChangeinCash")]
    public decimal? OtherCashAdjustmentOutsideChangeinCash { get; set; }

    [JsonPropertyName("otherCashPaymentsfromOperatingActivities")]
    public decimal? OtherCashPaymentsfromOperatingActivities { get; set; }

    [JsonPropertyName("otherCashReceiptsfromOperatingActivities")]
    public decimal? OtherCashReceiptsfromOperatingActivities { get; set; }

    [JsonPropertyName("otherCostofRevenue")]
    public decimal? OtherCostofRevenue { get; set; }

    [JsonPropertyName("otherCurrentAssets")]
    public decimal? OtherCurrentAssets { get; set; }

    [JsonPropertyName("otherCurrentBorrowings")]
    public decimal? OtherCurrentBorrowings { get; set; }

    [JsonPropertyName("otherCurrentLiabilities")]
    public decimal? OtherCurrentLiabilities { get; set; }

    [JsonPropertyName("otherCustomerServices")]
    public decimal? OtherCustomerServices { get; set; }

    [JsonPropertyName("otherEquityAdjustments")]
    public decimal? OtherEquityAdjustments { get; set; }

    [JsonPropertyName("otherEquityInterest")]
    public decimal? OtherEquityInterest { get; set; }

    [JsonPropertyName("otherGandA")]
    public decimal? OtherGandA { get; set; }

    [JsonPropertyName("otherIncomeExpense")]
    public decimal? OtherIncomeExpense { get; set; }

    [JsonPropertyName("otherIntangibleAssets")]
    public decimal? OtherIntangibleAssets { get; set; }

    [JsonPropertyName("otherInterestExpense")]
    public decimal? OtherInterestExpense { get; set; }

    [JsonPropertyName("otherInterestIncome")]
    public decimal? OtherInterestIncome { get; set; }

    [JsonPropertyName("otherInventories")]
    public decimal? OtherInventories { get; set; }

    [JsonPropertyName("otherInvestments")]
    public decimal? OtherInvestments { get; set; }

    [JsonPropertyName("otherLiabilities")]
    public decimal? OtherLiabilities { get; set; }

    [JsonPropertyName("otherLoanAssets")]
    public decimal? OtherLoanAssets { get; set; }

    [JsonPropertyName("otherNonCashItems")]
    public decimal? OtherNonCashItems { get; set; }

    [JsonPropertyName("otherNonCurrentAssets")]
    public decimal? OtherNonCurrentAssets { get; set; }

    [JsonPropertyName("otherNonCurrentLiabilities")]
    public decimal? OtherNonCurrentLiabilities { get; set; }

    [JsonPropertyName("otherNonInterestExpense")]
    public decimal? OtherNonInterestExpense { get; set; }

    [JsonPropertyName("otherNonInterestIncome")]
    public decimal? OtherNonInterestIncome { get; set; }

    [JsonPropertyName("otherNonOperatingIncomeExpenses")]
    public decimal? OtherNonOperatingIncomeExpenses { get; set; }

    [JsonPropertyName("otherOperatingExpenses")]
    public decimal? OtherOperatingExpenses { get; set; }

    [JsonPropertyName("otherPayable")]
    public decimal? OtherPayable { get; set; }

    [JsonPropertyName("otherProperties")]
    public decimal? OtherProperties { get; set; }

    [JsonPropertyName("otherRealEstateOwned")]
    public decimal? OtherRealEstateOwned { get; set; }

    [JsonPropertyName("otherReceivables")]
    public decimal? OtherReceivables { get; set; }

    [JsonPropertyName("otherShortTermInvestments")]
    public decimal? OtherShortTermInvestments { get; set; }

    [JsonPropertyName("otherSpecialCharges")]
    public decimal? OtherSpecialCharges { get; set; }

    [JsonPropertyName("otherTaxes")]
    public decimal? OtherTaxes { get; set; }

    [JsonPropertyName("otherThanPreferredStockDividend")]
    public decimal? OtherThanPreferredStockDividend { get; set; }

    [JsonPropertyName("otherunderPreferredStockDividend")]
    public decimal? OtherunderPreferredStockDividend { get; set; }

    [JsonPropertyName("payables")]
    public decimal? Payables { get; set; }

    [JsonPropertyName("payablesAndAccruedExpenses")]
    public decimal? PayablesAndAccruedExpenses { get; set; }

    [JsonPropertyName("paymentForLoans")]
    public decimal? PaymentForLoans { get; set; }

    [JsonPropertyName("paymentsonBehalfofEmployees")]
    public decimal? PaymentsonBehalfofEmployees { get; set; }

    [JsonPropertyName("paymentstoSuppliersforGoodsandServices")]
    public decimal? PaymentstoSuppliersforGoodsandServices { get; set; }

    [JsonPropertyName("pensionAndEmployeeBenefitExpense")]
    public decimal? PensionAndEmployeeBenefitExpense { get; set; }

    [JsonPropertyName("pensionandOtherPostRetirementBenefitPlansCurrent")]
    public decimal? PensionandOtherPostRetirementBenefitPlansCurrent { get; set; }

    [JsonPropertyName("periodType")]
    public string PeriodType { get; set; } = null!;

    [JsonPropertyName("preferredSecuritiesOutsideStockEquity")]
    public decimal? PreferredSecuritiesOutsideStockEquity { get; set; }

    [JsonPropertyName("preferredSharesNumber")]
    public decimal? PreferredSharesNumber { get; set; }

    [JsonPropertyName("preferredStock")]
    public decimal? PreferredStock { get; set; }

    [JsonPropertyName("preferredStockDividendPaid")]
    public decimal? PreferredStockDividendPaid { get; set; }

    [JsonPropertyName("preferredStockDividends")]
    public decimal? PreferredStockDividends { get; set; }

    [JsonPropertyName("preferredStockEquity")]
    public decimal? PreferredStockEquity { get; set; }

    [JsonPropertyName("preferredStockIssuance")]
    public decimal? PreferredStockIssuance { get; set; }

    [JsonPropertyName("preferredStockPayments")]
    public decimal? PreferredStockPayments { get; set; }

    [JsonPropertyName("prepaidAssets")]
    public decimal? PrepaidAssets { get; set; }

    [JsonPropertyName("pretaxIncome")]
    public decimal? PretaxIncome { get; set; }

    [JsonPropertyName("proceedsFromLoans")]
    public decimal? ProceedsFromLoans { get; set; }

    [JsonPropertyName("proceedsFromStockOptionExercised")]
    public decimal? ProceedsFromStockOptionExercised { get; set; }

    [JsonPropertyName("proceedsPaymentInInterestBearingDepositsInBank")]
    public decimal? ProceedsPaymentInInterestBearingDepositsInBank { get; set; }

    [JsonPropertyName("professionalExpenseAndContractServicesExpense")]
    public decimal? ProfessionalExpenseAndContractServicesExpense { get; set; }

    [JsonPropertyName("properties")]
    public decimal? Properties { get; set; }

    [JsonPropertyName("provisionForDoubtfulAccounts")]
    public decimal? ProvisionForDoubtfulAccounts { get; set; }

    [JsonPropertyName("provisionForLoanLeaseAndOtherLosses")]
    public decimal? ProvisionForLoanLeaseAndOtherLosses { get; set; }

    [JsonPropertyName("provisionandWriteOffofAssets")]
    public decimal? ProvisionandWriteOffofAssets { get; set; }

    [JsonPropertyName("purchaseOfBusiness")]
    public decimal? PurchaseOfBusiness { get; set; }

    [JsonPropertyName("purchaseOfIntangibles")]
    public decimal? PurchaseOfIntangibles { get; set; }

    [JsonPropertyName("purchaseOfInvestment")]
    public decimal? PurchaseOfInvestment { get; set; }

    [JsonPropertyName("purchaseOfInvestmentProperties")]
    public decimal? PurchaseOfInvestmentProperties { get; set; }

    [JsonPropertyName("purchaseOfPPE")]
    public decimal? PurchaseOfPPE { get; set; }

    [JsonPropertyName("rawMaterials")]
    public decimal? RawMaterials { get; set; }

    [JsonPropertyName("realizedGainLossOnSaleOfLoansAndLease")]
    public decimal? RealizedGainLossOnSaleOfLoansAndLease { get; set; }

    [JsonPropertyName("receiptsfromCustomers")]
    public decimal? ReceiptsfromCustomers { get; set; }

    [JsonPropertyName("receiptsfromGovernmentGrants")]
    public decimal? ReceiptsfromGovernmentGrants { get; set; }

    [JsonPropertyName("receivables")]
    public decimal? Receivables { get; set; }

    [JsonPropertyName("receivablesAdjustmentsAllowances")]
    public decimal? ReceivablesAdjustmentsAllowances { get; set; }

    [JsonPropertyName("reconciledCostOfRevenue")]
    public decimal? ReconciledCostOfRevenue { get; set; }

    [JsonPropertyName("reconciledDepreciation")]
    public decimal? ReconciledDepreciation { get; set; }

    [JsonPropertyName("rentAndLandingFees")]
    public decimal? RentAndLandingFees { get; set; }

    [JsonPropertyName("rentExpenseSupplemental")]
    public decimal? RentExpenseSupplemental { get; set; }

    [JsonPropertyName("repaymentOfDebt")]
    public decimal? RepaymentOfDebt { get; set; }

    [JsonPropertyName("reportedNormalizedBasicEPS")]
    public decimal? ReportedNormalizedBasicEPS { get; set; }

    [JsonPropertyName("reportedNormalizedDilutedEPS")]
    public decimal? ReportedNormalizedDilutedEPS { get; set; }

    [JsonPropertyName("repurchaseOfCapitalStock")]
    public decimal? RepurchaseOfCapitalStock { get; set; }

    [JsonPropertyName("researchAndDevelopment")]
    public decimal? ResearchAndDevelopment { get; set; }

    [JsonPropertyName("restrictedCash")]
    public decimal? RestrictedCash { get; set; }

    [JsonPropertyName("restrictedCashAndCashEquivalents")]
    public decimal? RestrictedCashAndCashEquivalents { get; set; }

    [JsonPropertyName("restrictedCashAndInvestments")]
    public decimal? RestrictedCashAndInvestments { get; set; }

    [JsonPropertyName("restrictedCommonStock")]
    public decimal? RestrictedCommonStock { get; set; }

    [JsonPropertyName("restrictedInvestments")]
    public decimal? RestrictedInvestments { get; set; }

    [JsonPropertyName("restructuringAndMergernAcquisition")]
    public decimal? RestructuringAndMergernAcquisition { get; set; }

    [JsonPropertyName("retainedEarnings")]
    public decimal? RetainedEarnings { get; set; }

    [JsonPropertyName("salariesAndWages")]
    public decimal? SalariesAndWages { get; set; }

    [JsonPropertyName("saleOfBusiness")]
    public decimal? SaleOfBusiness { get; set; }

    [JsonPropertyName("saleOfIntangibles")]
    public decimal? SaleOfIntangibles { get; set; }

    [JsonPropertyName("saleOfInvestment")]
    public decimal? SaleOfInvestment { get; set; }

    [JsonPropertyName("saleOfInvestmentProperties")]
    public decimal? SaleOfInvestmentProperties { get; set; }

    [JsonPropertyName("saleOfPPE")]
    public decimal? SaleOfPPE { get; set; }

    [JsonPropertyName("securitiesActivities")]
    public decimal? SecuritiesActivities { get; set; }

    [JsonPropertyName("securitiesAmortization")]
    public decimal? SecuritiesAmortization { get; set; }

    [JsonPropertyName("securitiesAndInvestments")]
    public decimal? SecuritiesAndInvestments { get; set; }

    [JsonPropertyName("securitiesLoaned")]
    public decimal? SecuritiesLoaned { get; set; }

    [JsonPropertyName("securityAgreeToBeResell")]
    public decimal? SecurityAgreeToBeResell { get; set; }

    [JsonPropertyName("securityBorrowed")]
    public decimal? SecurityBorrowed { get; set; }

    [JsonPropertyName("sellingAndMarketingExpense")]
    public decimal? SellingAndMarketingExpense { get; set; }

    [JsonPropertyName("sellingGeneralAndAdministration")]
    public decimal? SellingGeneralAndAdministration { get; set; }

    [JsonPropertyName("serviceChargeOnDepositorAccounts")]
    public decimal? ServiceChargeOnDepositorAccounts { get; set; }

    [JsonPropertyName("shareIssued")]
    public decimal? ShareIssued { get; set; }

    [JsonPropertyName("shortTermDebtIssuance")]
    public decimal? ShortTermDebtIssuance { get; set; }

    [JsonPropertyName("shortTermDebtPayments")]
    public decimal? ShortTermDebtPayments { get; set; }

    [JsonPropertyName("specialIncomeCharges")]
    public decimal? SpecialIncomeCharges { get; set; }

    [JsonPropertyName("stockBasedCompensation")]
    public decimal? StockBasedCompensation { get; set; }

    [JsonPropertyName("stockholdersEquity")]
    public decimal? StockholdersEquity { get; set; }

    [JsonPropertyName("subordinatedLiabilities")]
    public decimal? SubordinatedLiabilities { get; set; }

    [JsonPropertyName("tangibleBookValue")]
    public decimal? TangibleBookValue { get; set; }

    [JsonPropertyName("taxEffectOfUnusualItems")]
    public decimal? TaxEffectOfUnusualItems { get; set; }

    [JsonPropertyName("taxLossCarryforwardBasicEPS")]
    public decimal? TaxLossCarryforwardBasicEPS { get; set; }

    [JsonPropertyName("taxLossCarryforwardDilutedEPS")]
    public decimal? TaxLossCarryforwardDilutedEPS { get; set; }

    [JsonPropertyName("taxProvision")]
    public decimal? TaxProvision { get; set; }

    [JsonPropertyName("taxRateForCalcs")]
    public decimal? TaxRateForCalcs { get; set; }

    [JsonPropertyName("taxesReceivable")]
    public decimal? TaxesReceivable { get; set; }

    [JsonPropertyName("taxesRefundPaid")]
    public decimal? TaxesRefundPaid { get; set; }

    [JsonPropertyName("taxesRefundPaidDirect")]
    public decimal? TaxesRefundPaidDirect { get; set; }

    [JsonPropertyName("totalAssets")]
    public decimal? TotalAssets { get; set; }

    [JsonPropertyName("totalCapitalization")]
    public decimal? TotalCapitalization { get; set; }

    [JsonPropertyName("totalDebt")]
    public decimal? TotalDebt { get; set; }

    [JsonPropertyName("totalDeposits")]
    public decimal? TotalDeposits { get; set; }

    [JsonPropertyName("totalEquityGrossMinorityInterest")]
    public decimal? TotalEquityGrossMinorityInterest { get; set; }

    [JsonPropertyName("totalExpenses")]
    public decimal? TotalExpenses { get; set; }

    [JsonPropertyName("totalLiabilitiesNetMinorityInterest")]
    public decimal? TotalLiabilitiesNetMinorityInterest { get; set; }

    [JsonPropertyName("totalMoneyMarketInvestments")]
    public decimal? TotalMoneyMarketInvestments { get; set; }

    [JsonPropertyName("totalNonCurrentAssets")]
    public decimal? TotalNonCurrentAssets { get; set; }

    [JsonPropertyName("totalNonCurrentLiabilitiesNetMinorityInterest")]
    public decimal? TotalNonCurrentLiabilitiesNetMinorityInterest { get; set; }

    [JsonPropertyName("totalOperatingIncomeAsReported")]
    public decimal? TotalOperatingIncomeAsReported { get; set; }

    [JsonPropertyName("totalOtherFinanceCost")]
    public decimal? TotalOtherFinanceCost { get; set; }

    [JsonPropertyName("totalPartnershipCapital")]
    public decimal? TotalPartnershipCapital { get; set; }

    [JsonPropertyName("totalPremiumsEarned")]
    public decimal? TotalPremiumsEarned { get; set; }

    [JsonPropertyName("totalRevenue")]
    public decimal? TotalRevenue { get; set; }

    [JsonPropertyName("totalTaxPayable")]
    public decimal? TotalTaxPayable { get; set; }

    [JsonPropertyName("totalUnusualItems")]
    public decimal? TotalUnusualItems { get; set; }

    [JsonPropertyName("totalUnusualItemsExcludingGoodwill")]
    public decimal? TotalUnusualItemsExcludingGoodwill { get; set; }

    [JsonPropertyName("tradeandOtherPayablesNonCurrent")]
    public decimal? TradeandOtherPayablesNonCurrent { get; set; }

    [JsonPropertyName("tradingGainLoss")]
    public decimal? TradingGainLoss { get; set; }

    [JsonPropertyName("tradingLiabilities")]
    public decimal? TradingLiabilities { get; set; }

    [JsonPropertyName("tradingSecurities")]
    public decimal? TradingSecurities { get; set; }

    [JsonPropertyName("treasurySharesNumber")]
    public decimal? TreasurySharesNumber { get; set; }

    [JsonPropertyName("treasuryStock")]
    public decimal? TreasuryStock { get; set; }

    [JsonPropertyName("trustFeesbyCommissions")]
    public decimal? TrustFeesbyCommissions { get; set; }

    [JsonPropertyName("unearnedIncome")]
    public decimal? UnearnedIncome { get; set; }

    [JsonPropertyName("unrealizedGainLoss")]
    public decimal? UnrealizedGainLoss { get; set; }

    [JsonPropertyName("unrealizedGainLossOnInvestmentSecurities")]
    public decimal? UnrealizedGainLossOnInvestmentSecurities { get; set; }

    [JsonPropertyName("workInProcess")]
    public decimal? WorkInProcess { get; set; }

    [JsonPropertyName("workingCapital")]
    public decimal? WorkingCapital { get; set; }

    [JsonPropertyName("writeOff")]
    public decimal? WriteOff { get; set; }
}
