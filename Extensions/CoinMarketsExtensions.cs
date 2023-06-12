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
  public const string CurrencyTypeId = "t_currency";
  public const string CategoryTypeId = "c_coins";

  public static IEnumerable<IMoBroItem> MapToItems(this CoinMarkets cm)
  {
    // one group per coin
    var coinId = cm.Id;
    var groupId = $"g_{coinId}";
    yield return MoBroItem.CreateGroup()
      .WithId(groupId)
      .WithLabel(cm.Name)
      .Build();

    // all metrics of that coin
    yield return CoreMetric(groupId, coinId, "symbol", CoreMetricType.Text, true);
    yield return CoreMetric(groupId, coinId, "name", CoreMetricType.Text, true);
    yield return CurrMetric(groupId, coinId, "current_price");
    yield return CurrMetric(groupId, coinId, "market_cap");
    yield return CoreMetric(groupId, coinId, "market_cap_rank", CoreMetricType.Numeric);
    yield return CurrMetric(groupId, coinId, "total_volume");
    yield return CurrMetric(groupId, coinId, "high_24h");
    yield return CurrMetric(groupId, coinId, "low_24h");
    yield return CurrMetric(groupId, coinId, "price_change_24h");
    yield return CoreMetric(groupId, coinId, "price_change_percentage_24h", CoreMetricType.Usage);
    yield return CoreMetric(groupId, coinId, "circulating_supply", CoreMetricType.Numeric);
    yield return CoreMetric(groupId, coinId, "total_supply", CoreMetricType.Numeric);
    yield return CurrMetric(groupId, coinId, "ath");
    yield return CoreMetric(groupId, coinId, "ath_change_percentage", CoreMetricType.Usage);
    yield return CoreMetric(groupId, coinId, "ath_date", CoreMetricType.DateTime);
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

  private static IMetric CurrMetric(string groupId, string coinId, string key, bool isStatic = false) =>
    MoBroItem.CreateMetric()
      .WithId(MetricId(coinId, key))
      .WithLabel($"m_{key}_label", $"m_{key}_desc")
      .OfType(CurrencyTypeId)
      .OfCategory(CategoryTypeId)
      .OfGroup(groupId)
      .AsStaticValue(isStatic)
      .Build();

  private static IMetric CoreMetric(string groupId, string coinId, string key, CoreMetricType type,
    bool isStatic = false) =>
    MoBroItem.CreateMetric()
      .WithId(MetricId(coinId, key))
      .WithLabel($"m_{key}_label", $"m_{key}_desc")
      .OfType(type)
      .OfCategory(CategoryTypeId)
      .OfGroup(groupId)
      .AsStaticValue(isStatic)
      .Build();

  private static string MetricId(string coinId, string key) => $"m_{coinId}_{key}";
}