using System;
using System.Collections.Generic;
using CoinGecko.Entities.Response.Coins;
using MoBro.Plugin.SDK.Builders;
using MoBro.Plugin.SDK.Enums;
using MoBro.Plugin.SDK.Models;
using MoBro.Plugin.SDK.Models.Metrics;

namespace MoBro.Plugin.CoinGecko.Extensions;

public static class CoinMarketsExtensions
{
  public static IEnumerable<IMoBroItem> MapToItems(this CoinMarkets cm, string currency)
  {
    // one category per coin
    var coinId = cm.Id;
    var categoryId = $"{Ids.CategoryCoinsId}_{coinId}";
    yield return MoBroItem.CreateCategory()
      .WithId(categoryId)
      .WithLabel($"Coin: {cm.Name}")
      .Build();

    // parse currency type
    var currencyType = Enum.Parse<CoreMetricTypeCurrency>(currency.ToUpper());

    // all metrics of that coin
    yield return CoreMetric(categoryId, coinId, "symbol", CoreMetricType.Text, true);
    yield return CoreMetric(categoryId, coinId, "name", CoreMetricType.Text, true);
    yield return CurrMetric(categoryId, coinId, "current_price", currencyType);
    yield return CurrMetric(categoryId, coinId, "market_cap", currencyType);
    yield return CoreMetric(categoryId, coinId, "market_cap_rank", CoreMetricType.Numeric);
    yield return CurrMetric(categoryId, coinId, "total_volume", currencyType);
    yield return CurrMetric(categoryId, coinId, "high_24h", currencyType);
    yield return CurrMetric(categoryId, coinId, "low_24h", currencyType);
    yield return CurrMetric(categoryId, coinId, "price_change_24h", currencyType);
    yield return CoreMetric(categoryId, coinId, "price_change_percentage_24h", CoreMetricType.Usage);
    yield return CoreMetric(categoryId, coinId, "circulating_supply", CoreMetricType.Numeric);
    yield return CoreMetric(categoryId, coinId, "total_supply", CoreMetricType.Numeric);
    yield return CurrMetric(categoryId, coinId, "ath", currencyType);
    yield return CoreMetric(categoryId, coinId, "ath_change_percentage", CoreMetricType.Usage);
    yield return CoreMetric(categoryId, coinId, "ath_date", CoreMetricType.DateTime);
  }

  public static IEnumerable<MetricValue> MapToMetricValues(this CoinMarkets cm)
  {
    // all metric values of the coin
    yield return new MetricValue(MetricId(cm.Id, "symbol"), cm.GetLastUpdated(), cm.Symbol);
    yield return new MetricValue(MetricId(cm.Id, "name"), cm.GetLastUpdated(), cm.Name);
    yield return new MetricValue(MetricId(cm.Id, "current_price"), cm.GetLastUpdated(), cm.CurrentPrice);
    yield return new MetricValue(MetricId(cm.Id, "market_cap"), cm.GetLastUpdated(), cm.MarketCap);
    yield return new MetricValue(MetricId(cm.Id, "market_cap_rank"), cm.GetLastUpdated(), cm.MarketCapRank);
    yield return new MetricValue(MetricId(cm.Id, "total_volume"), cm.GetLastUpdated(), cm.TotalVolume);
    yield return new MetricValue(MetricId(cm.Id, "high_24h"), cm.GetLastUpdated(), cm.High24H);
    yield return new MetricValue(MetricId(cm.Id, "low_24h"), cm.GetLastUpdated(), cm.Low24H);
    yield return new MetricValue(MetricId(cm.Id, "price_change_24h"), cm.GetLastUpdated(), cm.PriceChange24H);
    yield return new MetricValue(MetricId(cm.Id, "price_change_percentage_24h"), cm.GetLastUpdated(), cm.PriceChangePercentage24H);
    yield return new MetricValue(MetricId(cm.Id, "circulating_supply"), cm.GetLastUpdated(), cm.CirculatingSupply);
    yield return new MetricValue(MetricId(cm.Id, "total_supply"), cm.GetLastUpdated(), cm.TotalSupply);
    yield return new MetricValue(MetricId(cm.Id, "ath"), cm.GetLastUpdated(), cm.Ath);
    yield return new MetricValue(MetricId(cm.Id, "ath_change_percentage"), cm.GetLastUpdated(), cm.AthChangePercentage);
    yield return new MetricValue(MetricId(cm.Id, "ath_date"), cm.GetLastUpdated(), cm.AthDate);
  }

  private static DateTime GetLastUpdated(this CoinMarkets coinMarkets) =>
    coinMarkets.LastUpdated?.UtcDateTime ?? DateTime.UtcNow;

  private static Metric CurrMetric(string categoryId, string coinId, string key, CoreMetricTypeCurrency currency) =>
    MoBroItem.CreateMetric()
      .WithId(MetricId(coinId, key))
      .WithLabel($"m_coins_{key}_label", $"m_coins_{key}_desc")
      .OfType(currency)
      .OfCategory(categoryId)
      .OfNoGroup()
      .AsDynamicValue()
      .Build();

  private static Metric CoreMetric(string categoryId, string coinId, string key, CoreMetricType type,
    bool isStatic = false) =>
    MoBroItem.CreateMetric()
      .WithId(MetricId(coinId, key))
      .WithLabel($"m_coins_{key}_label", $"m_coins_{key}_desc")
      .OfType(type)
      .OfCategory(categoryId)
      .OfNoGroup()
      .AsStaticValue(isStatic)
      .Build();

  private static string MetricId(string coinId, string key) => $"m_coins_{coinId}_{key}";
}